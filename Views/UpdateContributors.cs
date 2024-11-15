using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Plugin_ICGFront.Models;

namespace Plugin_ICGFront.Views
{
    public partial class UpdateContributors : BaseForm
    {
        private readonly ParsedDocument _parsedDocument;

        public UpdateContributors(ParsedDocument parsedDocument)
        {
            InitializeComponent();

            this._parsedDocument = parsedDocument;
        }

        private void DowloadFileLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://dgii.gov.do/app/WebApps/Consultas/RNC/DGII_RNC.zip");
        }

        private void SelectFile_Click(object sender, EventArgs e)
        {
            SelectedFilePath.Text = "";
            TextFileDialog.ShowDialog();
        }

        private void TextFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            var selectedFile = TextFileDialog.FileName;

            BeginInvoke((Action)(() => ProcessFile(selectedFile)));
        }

        private void ProcessFile(string selectedFile)
        {
            SelectFile.Enabled = false;

            try
            {
                SelectedFilePath.Text = selectedFile;

                var contributorRows = File.ReadAllLines(selectedFile);

                if (contributorRows.Length == 0)
                {
                    Utilities.ShowAlert("El archivo de texto esta vacio.", "Error", MessageBoxIcon.Error);
                    return;
                }

                var totalRows = contributorRows.Length + 1;

                ToolStripProgressBar.Maximum = totalRows;
                ToolStripProgressBar.Value = 0;

                var updateContributorsList = new List<ContributorInfo>();

                foreach (var rawLine in contributorRows)
                {
                    ToolStripProgressBar.PerformStep();
                    var currentRow = ToolStripProgressBar.Value;

                    ToolStripStatusLabel.Text = $@"Procesando linea {currentRow} de {totalRows}...";

                    var parsedContributor = ParseTextFileLine(rawLine);

                    if (parsedContributor != null) updateContributorsList.Add(parsedContributor);
                }

                var updateConfirmationResult = Utilities.ShowAlert(
                    $@"El archivo contiene {updateContributorsList.Count} contribuyentes activos. ¿Desea actualizar la base de datos?",
                    "Atencion",
                    MessageBoxIcon.Question,
                    MessageBoxButtons.YesNo
                );

                if (updateConfirmationResult != DialogResult.Yes) return;

                totalRows = updateContributorsList.Count;

                ToolStripProgressBar.Maximum = totalRows;
                ToolStripProgressBar.Value = 0;

                if (!CreateTableSchema())
                {
                    Utilities.ShowAlert("No se pudo crear la tabla para almacenar los contribuyentes.", "Error",
                        MessageBoxIcon.Error);
                    return;
                }

                foreach (var contributor in updateContributorsList)
                {
                    ToolStripProgressBar.PerformStep();
                    var currentRow = ToolStripProgressBar.Value;

                    ToolStripStatusLabel.Text = $@"Procesando linea {currentRow} de {totalRows}...";

                    if (!InsertNewContributor(contributor))
                    {
                        Utilities.ShowAlert(
                            $@"No se pudo insertar el contribuyente '{contributor.Rnc}'. Verifique que el usuario '{_parsedDocument.UserName}' posea los permisos correspondientes para la base de datos '{_parsedDocument.DatabaseName}'.",
                            "Error",
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                }

                Utilities.ShowAlert("Los contribuyentes fueron actualizados exitosamente.");
                Close();
            }
            catch (IOException ex)
            {
                Utilities.ShowAlert(ex.Message, "Error", MessageBoxIcon.Error);
                Close();
            }
            catch (Exception)
            {
                ToolStripProgressBar.Maximum = 1;
                ToolStripProgressBar.Value = 1;
                ToolStripStatusLabel.Text = @"Ocurrio un error al procesar el archivo.";
            }
            finally
            {
                ToolStripProgressBar.Value = 0;
                ToolStripProgressBar.Maximum = 1;
                ToolStripStatusLabel.Text = "";

                SelectFile.Enabled = true;
            }
        }

        private ContributorInfo ParseTextFileLine(string textFileLine)
        {
            try
            {
                var parsedLine = textFileLine.Split('|');

                if (!parsedLine[9].Equals("ACTIVO") || parsedLine[9].Equals("NORMAL")) return null;

                return new ContributorInfo
                {
                    Rnc = parsedLine[0],
                    Name = parsedLine[1],
                    CommercialName = parsedLine[2],
                    Category = parsedLine[3],
                    PaymentScheme = "2",
                    Status = "2"
                };
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        private bool CreateTableSchema()
        {
            var sqlScript = @"
            IF OBJECT_ID(N'dbo.DGII_CONTRIBUYENTES', N'U') IS NOT NULL
                DROP TABLE [dbo].[DGII_CONTRIBUYENTES];

            CREATE TABLE [dbo].[DGII_CONTRIBUYENTES] (
	            [rnc] [nvarchar](100) NOT NULL,
	            [name] [nvarchar](250) NOT NULL,
	            [commercialName] [nvarchar](250) NULL,
	            [category] [nvarchar](250) NULL,
	            [paymentScheme] [nvarchar](2) NOT NULL,
	            [status] [nvarchar](50) NOT NULL,
	            CONSTRAINT [PK_DGII_CONTRIBUYENTES] PRIMARY KEY CLUSTERED ([rnc] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]".Replace("\n", "").Replace("\r", "");

            try
            {
                using (var connection = _parsedDocument.GetSqlConnection())
                using (var commandForTableSchema = new SqlCommand(sqlScript, connection))
                {
                    connection.Open();
                    commandForTableSchema.ExecuteNonQuery();
                    connection.Close();

                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }

        private bool InsertNewContributor(ContributorInfo contributor)
        {
            var sqlScript = @"
            INSERT INTO [dbo].[DGII_CONTRIBUYENTES] (
                rnc,
                name,
                commercialName,
                category,
                paymentScheme,
                status
            ) VALUES (
                @rnc,
                @name,
                @commercialName,
                @category,
                @paymentScheme,
                @status
            );".Replace("\n", "").Replace("\r", "");

            try
            {
                using (var connection = _parsedDocument.GetSqlConnection())
                using (var commandForInsertRow = new SqlCommand(sqlScript, connection))
                {
                    connection.Open();

                    commandForInsertRow.Parameters.AddWithValue("@rnc", SqlDbType.NVarChar).Value = contributor.Rnc;
                    commandForInsertRow.Parameters.AddWithValue("@name", SqlDbType.NVarChar).Value = contributor.Name;
                    commandForInsertRow.Parameters.AddWithValue("@commercialName", SqlDbType.NVarChar).Value =
                        contributor.CommercialName;
                    commandForInsertRow.Parameters.AddWithValue("@category", SqlDbType.NVarChar).Value =
                        contributor.Category;
                    commandForInsertRow.Parameters.AddWithValue("@paymentScheme", SqlDbType.NVarChar).Value =
                        contributor.PaymentScheme;
                    commandForInsertRow.Parameters.AddWithValue("@status", SqlDbType.NVarChar).Value =
                        contributor.Status;

                    commandForInsertRow.ExecuteNonQuery();
                    connection.Close();

                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }
    }
}