using Plugin_ICGFront.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Plugin_ICGFront.Views
{

    public partial class ClearPendingDocuments : Form
    {
        private readonly ParsedDocument _parsedDocument;

        List<InvoicePendingInfo>_invoicePendingInfo;

        private Timer timer;

        bool valida_hay_documentos;
        
        DateTime hoy;
        string dia_servidor;

        public ClearPendingDocuments(ParsedDocument parsedDocument)
        { 
             
            InitializeComponent();
            InitializeTimer();
            this._parsedDocument = parsedDocument;

            try
            {
                this.lb_nombre_trabajador.Text = this._parsedDocument.GetEmployeeName();
            }
            catch(Exception ex)
            {
                this.lb_nombre_trabajador.Text = "Not found";
                MessageBox.Show("Error Buscando Nombre Trabajador: "+ex);
            }
            
            try
            {
                this.dataGridView1.Columns.Add("NUMALBARAN", "NÚMERO ALBARAN");
                this.dataGridView1.Columns.Add("NUMSERIE", "NÚMERO SERIE");
                this.dataGridView1.Columns.Add("NOMBRECLIENTE", "CLIENTE");
                this.dataGridView1.Columns.Add("TOTALBRUTO", "TOTAL BRUTO");
                this.dataGridView1.Columns.Add("TOTALNETO", "TOTAL NETO");
                this.dataGridView1.Columns.Add("CAJA", "CAJA");
                this.dataGridView1.Columns.Add("CODVENDEDOR", "CODIGO VENDEDOR");
                this.dataGridView1.Columns.Add("NOMVENDEDOR", "VENDEDOR DESPACHO");
                this.dataGridView1.Columns.Add("FECHA", "FECHA");


                List<InvoicePendingInfo> documentosPendientes = this._parsedDocument.GetPendingDocuments();
                if (documentosPendientes.Count > 0)
                {
                    foreach (var persona in documentosPendientes)
                    {
                        dataGridView1.Rows.Add
                            (
                                persona.NUMALBARAN, 
                                persona.NUMSERIE, 
                                persona.NOMBRECLIENTE, 
                                persona.TOTALBRUTO, 
                                persona.TOTALNETO, 
                                persona.CAJA,
                                persona.CODVENDEDOR,
                                persona.NOMVENDEDOR,
                                persona.FECHA

                            );
                    }
                    this.valida_hay_documentos = true;
                }
                else
                {
                    this.valida_hay_documentos = false;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, No se encontraron documentos pendientes.");
            }
        }

        private void InitializeTimer()
        {
            this.MaximizeBox = false; 
            this.MinimizeBox = false;
            timer = new Timer();
            timer.Interval = 1000; // Intervalo en milisegundos (1 segundo)
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
#if !DEBUG
            using (var command = this._parsedDocument.RecuperaHoraServidorCmd())
            { 
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string currentDateTime = reader.GetString(0);
                    this.lblRelog.Text = currentDateTime;
                    this.hoy = Convert.ToDateTime(currentDateTime);
                }

                reader.Close();
            }
#endif
        }



        private void continueBtn_Click(object sender, EventArgs e)
        {
            bool puede_eliminar_facturas;
            DateTime hora_valida = DateTime.Today.Add(new TimeSpan(18, 0, 0));

            try
            {
                puede_eliminar_facturas = this._parsedDocument.CanDeleteInvoices();
            }catch(Exception ex)
            {
                puede_eliminar_facturas = false;
                Utilities.ShowAlert("Error: "+ex, "Error", MessageBoxIcon.Error);

            }
            try
            {
                this.dia_servidor = this._parsedDocument.RecuperaDiaServidor();
            }
            catch (Exception ex)
            {
                Utilities.ShowAlert("Error: " + ex, "Error", MessageBoxIcon.Error);
            }

            if (valida_hay_documentos)
            {
                if (this.dia_servidor == "Saturday" || this.dia_servidor == "saturday" || this.dia_servidor == "Sábado" || this.dia_servidor == "Sabado" || this.dia_servidor == "SABADO" 
                    || this.dia_servidor == "sabado")
                {
                    if ((this.hoy.Hour >= 11 && this.hoy.Minute >= 50 || (this.hoy.Hour>=12)) || (this.hoy.Hour <= 6))
                    {
                        if (!puede_eliminar_facturas)
                        {
                            Utilities.ShowAlert("No tienes permisos suficientes para realizar esta acción.", "Error", MessageBoxIcon.Error);
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Esta acción es IRREVERSIBLE, ¿estás seguro?", "Advertencia", MessageBoxButtons.YesNo);

                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    this._parsedDocument.AuditDeletedPendingInvoices(this.lb_nombre_trabajador.Text.Trim());
                                    this._parsedDocument.DeletePendingDocuments();
                                    Utilities.ShowAlert("Éxito!");
                                    Close();
                                }
                                catch (Exception ex)
                                {
                                    Utilities.ShowAlert("Algo salió mal, vuelva a intentarlo más tarde. " + ex, "Error", MessageBoxIcon.Error);
                                    Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        Utilities.ShowAlert("Aún no se puede realizar esta acción, hora mínima del servidor: 11:50 am.", "Error", MessageBoxIcon.Error);
                    }
                }
                else
                {
                    //MessageBox.Show("Día Servidor: "+this.dia_servidor+ " Hora Servidor: "+this.hoy.Hour+" y minuto: "+this.hoy.Minute);


                    if (((this.hoy.Hour >= 17 && this.hoy.Minute >= 50) || (this.hoy.Hour >= 18)) || (this.hoy.Hour < 6))
                    {
                        if (!puede_eliminar_facturas)
                        {
                            Utilities.ShowAlert("No tienes permisos suficientes para realizar esta acción.", "Error", MessageBoxIcon.Error);
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Esta acción es IRREVERSIBLE, ¿estás seguro?", "Advertencia", MessageBoxButtons.YesNo);

                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    this._parsedDocument.AuditDeletedPendingInvoices(this.lb_nombre_trabajador.Text.Trim());
                                    this._parsedDocument.DeletePendingDocuments();
                                    Utilities.ShowAlert("ÉXITO!");
                                    Close();
                                }
                                catch (Exception ex)
                                {
                                    Utilities.ShowAlert("Algo salió mal, vuelva a intentarlo más tarde. " + ex, "Error", MessageBoxIcon.Error);
                                    Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        Utilities.ShowAlert("Aún no se puede realizar esta acción, hora mínima del servidor: 5:50pm.", "Error", MessageBoxIcon.Error);
                    }
                    
                }
                
            }
            else
            {
                Utilities.ShowAlert("No hay documentos en espera.", "Error", MessageBoxIcon.Error);
            }
        }
         
        private void Cancel_Btn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ClearPendingDocuments_Load(object sender, EventArgs e)
        {

        }
    }

    public class Datos
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
    }
}
