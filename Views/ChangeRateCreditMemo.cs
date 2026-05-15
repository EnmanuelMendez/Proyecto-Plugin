using System;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class ChangeRateCreditMemo : Form
    {
        private readonly ParsedDocument _parsedDocument;
        private decimal tasa_sistema = -1;
        private decimal? tasa_factura = -1;

        public ChangeRateCreditMemo(ParsedDocument parsedDocument)
        {
            this._parsedDocument = parsedDocument;
            InitializeComponent();

            this.MaximizeBox = false;
            this.MinimizeBox = false;

            //obtener tasa del sistema y de la factura
            ObtenerTasas();

            btn_cambiar_tasa.Click += btn_aceptar_Click;
            btn_cancelar.Click += btn_cancelar_Click;
        }

        private void ObtenerTasas()
        {
            this.tasa_sistema = this._parsedDocument.GetCurrencyRate();
            this.tasa_factura = this._parsedDocument.GetInvoiceRate();

            if (this.tasa_factura == null)
            {
                MessageBox.Show(
                    "No se encontró una factura con esa serie y número.",
                    "Factura no encontrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            this.lb_pregunta.Text = $"¿DESEA CAMBIAR LA TASA DEL SISTEMA A? {tasa_factura.Value:N4}?";

            this.txt_tasa_sistema.Text = tasa_sistema.ToString();
            this.txt_tasa_factura.Text = tasa_factura.ToString();
        }

        private void btn_aceptar_Click(object sender, EventArgs e)
        {
            if (!HorarioValido())
            {
                return;
            }

            string serie = txt_tasa_sistema.Text.Trim();
            string numeroTexto = txt_tasa_factura.Text.Trim();

            if (string.IsNullOrWhiteSpace(serie))
            {
                MessageBox.Show("Debe indicar la serie de la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_tasa_sistema.Focus();
                return;
            }

            if (!int.TryParse(numeroTexto, out int numero))
            {
                MessageBox.Show("Debe indicar un número de factura válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_tasa_factura.Focus();
                return;
            }

            try
            {
                decimal? tasaFactura = _parsedDocument.GetInvoiceRate(serie, numero);

                if (tasaFactura == null)
                {
                    MessageBox.Show(
                        "No se encontró una factura con esa serie y número.",
                        "Factura no encontrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                decimal tasaActualMoneda = _parsedDocument.GetCurrencyRate();
                decimal tasaActualCotizacion = _parsedDocument.GetTodayQuotationRate();

                string mensaje =
                    "Factura encontrada.\n\n" +
                    $"Serie: {serie}\n" +
                    $"Número: {numero}\n\n" +
                    $"Tasa actual en MONEDAS: {tasaActualMoneda:N4}\n" +
                    $"Tasa actual en COTIZACIONES: {tasaActualCotizacion:N4}\n" +
                    $"Tasa de la factura: {tasaFactura.Value:N4}\n\n" +
                    $"¿Desea cambiar la tasa actual a {tasaFactura.Value:N4}?";

                DialogResult result = MessageBox.Show(
                    mensaje,
                    "Confirmar cambio de tasa",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

                if (result != DialogResult.OK)
                {
                    txt_tasa_sistema.Focus();
                    return;
                }

                bool actualizado = _parsedDocument.ChangeRateForCreditMemo(tasaFactura.Value);

                if (actualizado)
                {
                    MessageBox.Show(
                        "La tasa fue actualizada correctamente en MONEDAS y COTIZACIONES.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo actualizar la tasa.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al cambiar la tasa: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool HorarioValido()
        {
            DateTime fechaServidor;

            try
            {
                fechaServidor = _parsedDocument.GetServerDateTime();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo validar la hora del servidor: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return false;
            }

            DayOfWeek dia = fechaServidor.DayOfWeek;
            TimeSpan hora = fechaServidor.TimeOfDay;

            bool esSabado = dia == DayOfWeek.Saturday;
            bool esDomingo = dia == DayOfWeek.Sunday;

            if (esDomingo)
            {
                MessageBox.Show(
                    "Esta acción no está permitida los domingos.",
                    "Horario no permitido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            if (esSabado)
            {
                bool horarioSabado =
                    hora >= new TimeSpan(11, 50, 0) &&
                    hora <= new TimeSpan(23, 59, 59);

                if (!horarioSabado)
                {
                    MessageBox.Show(
                        "Aún no se puede realizar esta acción. Los sábados se permite desde las 11:50 a.m. hasta las 11:59 p.m.",
                        "Horario no permitido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return false;
                }

                return true;
            }

            bool esLunesAViernes =
                dia >= DayOfWeek.Monday &&
                dia <= DayOfWeek.Friday;

            if (esLunesAViernes)
            {
                bool horarioLunesAViernes =
                    hora >= new TimeSpan(17, 55, 0) ||
                    hora <= new TimeSpan(5, 0, 0);

                if (!horarioLunesAViernes)
                {
                    MessageBox.Show(
                        "Aún no se puede realizar esta acción. De lunes a viernes se permite después de las 5:55 p.m. y antes de las 5:00 a.m.",
                        "Horario no permitido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return false;
                }

                return true;
            }

            return false;
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txt_serie_TextChanged(object sender, EventArgs e)
        {
        }

        private void btn_cambiar_tasa_Click(object sender, EventArgs e)
        {
            String mensaje = $"¿Desea cambiar la tasa actual a {this.txt_tasa_factura.Text:N4}?";

            DialogResult result = MessageBox.Show(
                    mensaje,
                    "Confirmar cambio de tasa",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

            if (result != DialogResult.OK)
            {
                txt_tasa_sistema.Focus();
                return;
            }
            try
            {
                bool actualizado = _parsedDocument.ChangeRateForCreditMemo(this.tasa_factura.Value);

                if (actualizado)
                {
                    MessageBox.Show(
                        "La tasa fue actualizada correctamente en MONEDAS y COTIZACIONES.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo actualizar la tasa.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al cambiar la tasa: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}