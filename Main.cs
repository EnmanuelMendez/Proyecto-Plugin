using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Plugin_ICGFront.Models;
using Plugin_ICGFront.Reports;
using Plugin_ICGFront.Views;
using WindowsInput;
using WindowsInput.Native;

namespace Plugin_ICGFront
{
    public partial class Main : Form
    {
        private readonly string _eventName;

        public Main(string eventName)
        {
            this._eventName = eventName;
            InitializeComponent();
        }

        [DllImport("USER32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Main_Load(object sender, EventArgs e)
        {
            var parsedDocument = Utilities.ParseXml();

            if (parsedDocument == null)
            {
                Application.Exit();
                return;
            }

            switch (_eventName)
            {
                case "reprint-raffle-tickets":
                    ReprintRaffleTickets_Event(parsedDocument);
                    break;
                case "print-raffle-tickets":
                case "print-invoice": // Deprecated
                    PrintRaffleTickets_Event(parsedDocument);
                    break;
                case "audit-invoice":
                    AuditInvoice_Event(parsedDocument);
                    break;
                case "send-esc":
                    SendEsc_Event();
                    break;
                case "open-calc":
                    OpenCalc_Event();
                    break;
                case "paste":
                    PasteSN_Event();
                    break;
                case "on-document-save":
                    OnDocumentSave_Event(parsedDocument);
                    break;
                case "pending-sale":
                    PendingSale_Event(parsedDocument);
                    break;
                case "audit-quote":
                    AuditQuote_Event(parsedDocument);
                    break;
                case "show-price-errors":
                    ShowPriceErrors_Event(parsedDocument);
                    break;
                case "update-contributors":
                    UpdateContributors_Event(parsedDocument);
                    break;
                case "show-current-rate":
                    ShowCurrentRate_Event(parsedDocument);
                    break;
                case "audit-documents":
                    AuditDocuments_Event(parsedDocument);
                    break;
                case "clear-pending-documents":
                    ClearPendingDocuments(parsedDocument);
                    break;
                case "saving-long-serials":
                    SavingLongSerials(parsedDocument);
                    break;
                default:
                    InvalidEventName_Event();
                    break;
            }
        }

        private void PasteSN_Event()
        {
            using (Form form = new PasteSerialNumbers())
            {
                form.ShowDialog();
            }

            Close();
        }

        private void EasterEggButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"This is still a WIP.");
        }

        private void ReprintRaffleTickets_Event(ParsedDocument parsedDocument)
        {
            using (var form = new PromptDocument(@"Reimprimir boletos de rifa"))
            {
                var dialogResult = form.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    parsedDocument.DocumentSeries = form.DocumentSeries;
                    parsedDocument.DocumentNumber = form.DocumentNumber;

                    PrintRaffleTickets_Event(parsedDocument, true);
                }
            }

            Close();
        }

        private void PrintRaffleTickets_Event(ParsedDocument parsedDocument, bool rePrint = false)
        {
            var raffleTickets = !rePrint ? parsedDocument.GenerateRaffleTickets() : parsedDocument.GetRaffleTickets();

            if (raffleTickets == null || raffleTickets.Count == 0)
            {
                if (rePrint)
                    MessageBox.Show(
                        $@"No se encontraron boletos para la factura '{parsedDocument.DocumentSeries}-{parsedDocument.DocumentNumber}'.");

                Close();
                return;
            }

            var printUsingFiscalPrinter = Utilities.PrintRaffleTicketsThroughFiscalPrinter();

            bool retryPrint;
            if (!printUsingFiscalPrinter)
            {
                do
                {
                    retryPrint = false;

                    try
                    {
                        const string password = "masterpwd";

                        var report = new raffle_tickets_report();
                        report.Load();

                        report.SetDatabaseLogon(
                            parsedDocument.UserName,
                            password,
                            parsedDocument.ServerName,
                            parsedDocument.DatabaseName
                        );

                        foreach (IConnectionInfo connectionInfo in report.DataSourceConnections)
                        {
                            connectionInfo.SetConnection(parsedDocument.ServerName, parsedDocument.DatabaseName, false);
                            connectionInfo.SetLogon(parsedDocument.UserName, password);
                        }

                        foreach (Table table in report.Database.Tables)
                        {
                            var logOnInfo = new TableLogOnInfo
                            {
                                ConnectionInfo =
                                {
                                    UserID = parsedDocument.UserName,
                                    DatabaseName = parsedDocument.DatabaseName,
                                    ServerName = parsedDocument.ServerName,
                                    Password = password
                                }
                            };

                            table.ApplyLogOnInfo(logOnInfo);
                        }

                        report.SetParameterValue("InvoiceSeries", parsedDocument.DocumentSeries);
                        report.SetParameterValue("InvoiceNumber", parsedDocument.DocumentNumber);

                        var printDialog = new PrintDialog
                        {
                            AllowSomePages = false,
                            AllowSelection = false,
                            AllowPrintToFile = false,
                            AllowCurrentPage = false,
                            ShowHelp = false
                        };

                        var printDialogResult = printDialog.ShowDialog(this);

                        if (printDialogResult == DialogResult.OK)
                        {
                            int copies = printDialog.PrinterSettings.Copies;
                            var fromPage = printDialog.PrinterSettings.FromPage;
                            var toPage = printDialog.PrinterSettings.ToPage;
                            var collate = printDialog.PrinterSettings.Collate;

                            report.PrintOptions.PrinterName = printDialog.PrinterSettings.PrinterName;
                            report.PrintToPrinter(copies, collate, fromPage, toPage);
                        }
                        else
                        {
                            report.Dispose();
                            printDialog.Dispose();

                            Close();
                            return;
                        }

                        report.Dispose();
                        printDialog.Dispose();
                    }
                    catch (Exception ex)
                    {
                        var promptResult = MessageBox.Show(
                            $@"Error al imprimir tickets. ¿Desea reintentar?{Environment.NewLine}{Environment.NewLine}--- Error ---{Environment.NewLine}{ex.Message}",
                            "Error",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1
                        );

                        retryPrint = promptResult == DialogResult.Retry;
                    }
                } while (retryPrint);

                Close();
                return;
            }

            try
            {
                var portName = Utilities.GetFiscalPrinterPort();
                var fiscalPrinter = new FiscalPrinter(portName, 9600);

                fiscalPrinter.InitConnection();

                foreach (var raffleTicket in raffleTickets)
                {
                    do
                    {
                        retryPrint = false;

                        var printed = PrintRaffleTicket(fiscalPrinter, raffleTicket);

                        if (printed) continue;
                        var promptResult = MessageBox.Show(
                            $@"No se pudo imprimir el boleto para la rifa '{raffleTicket.PromotionName}'. ¿Desea reintentar?",
                            "Error",
                            MessageBoxButtons.RetryCancel
                        );

                        retryPrint = promptResult == DialogResult.Retry;
                    } while (retryPrint);
                }

                fiscalPrinter.DeInitConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Error al imprimir tickets por la impresora fiscal: {ex.Message}");
            }

            Close();
        }

        private static bool PrintRaffleTicket(FiscalPrinter fiscalPrinter, RaffleTicket raffleTicket)
        {
            fiscalPrinter.OpenNonFiscalReceipt();

            fiscalPrinter.PrintNonFiscalText($@"{raffleTicket.PromotionName}", true, false, true, true);
            fiscalPrinter.PrintNonFiscalText("");
            fiscalPrinter.PrintNonFiscalText($@"{raffleTicket.Code}", false, false, false, false, false, true);
            fiscalPrinter.PrintNonFiscalText("");
            fiscalPrinter.PrintNonFiscalText(
                $@"Promocion del {raffleTicket.PromotionStartDate:dd/MM/yyyy} al {raffleTicket.PromotionEndDate:dd/MM/yyyy}");
            fiscalPrinter.PrintNonFiscalText($@"{raffleTicket.PromotionInfo}", false, true);
            fiscalPrinter.PrintNonFiscalText("");
            fiscalPrinter.PrintNonFiscalText($@"Generado en {raffleTicket.InvoiceSeries}-{raffleTicket.InvoiceNumber}");
            fiscalPrinter.PrintNonFiscalText("");

            fiscalPrinter.CloseNonFiscalReceipt();
            fiscalPrinter.PaperCut();

            Thread.Sleep(2000);

            return true;
        }

        private void AuditInvoice_Event(ParsedDocument parsedDocument)
        {
            var canCreateInvoices = parsedDocument.CanCreateInvoices();

            if (!canCreateInvoices)
            {
                StopTransactionExecution(parsedDocument);
                return;
            }

            var hasPriceErrors = parsedDocument.HasPriceErrors();
            var hasDocumentCurrencyMismatch = parsedDocument.HasDocumentCurrencyMismatch();

            //SAVING SERIALS HERE
            if (hasPriceErrors || hasDocumentCurrencyMismatch)
            {
                using (Form form = new AuditInvoice(_eventName, hasPriceErrors, hasDocumentCurrencyMismatch))
                {
                    form.ShowDialog();
                }

                StopTransactionExecution(parsedDocument);
                InvokeEventInNewProcess("show-price-errors");
                return;
            }

            var refundDetail = parsedDocument.InvoiceIsGreaterThan30Days();

            if (refundDetail != null && refundDetail.GreaterThan30Days)
            {
                parsedDocument.RemoveItbisFromRefund();

                using (Form form = new RefundAlert(_eventName, refundDetail))
                {
                    form.ShowDialog();
                }
            }

            var askForClientData = parsedDocument.CheckIfCustomClientInvoice();

            if (askForClientData)
                using (Form form = new CustomClient(parsedDocument))
                {
                    form.ShowDialog();

                    if (form.DialogResult != DialogResult.OK)
                    {
                        StopTransactionExecution(parsedDocument);
                        return;
                    }
                }

            var taxReceiptInfo = parsedDocument.GetTaxReceiptInfo();

            if (taxReceiptInfo == null)
            {
                Utilities.ShowAlert(
                    $@"No se pudo obtener la informacion del sistema para el documento '{parsedDocument.DocumentSeries}-{parsedDocument.DocumentNumber}'.",
                    "Error", MessageBoxIcon.Error);
                StopTransactionExecution(parsedDocument);
                return;
            }

            if (!taxReceiptInfo.Active)
            {
                Utilities.ShowAlert($@"El tipo de comprobante '{taxReceiptInfo.ReceiptType}' esta desactivado.",
                    "Error", MessageBoxIcon.Error);
                StopTransactionExecution(parsedDocument);
                return;
            }

            if (!taxReceiptInfo.NotExpired)
            {
                Utilities.ShowAlert($@"El tipo de comprobante '{taxReceiptInfo.ReceiptType}' esta expirado.", "Error",
                    MessageBoxIcon.Error);
                StopTransactionExecution(parsedDocument);
                return;
            }

            if (taxReceiptInfo.CustomerType == "02" && taxReceiptInfo.NetTotal >= 250000)
            {
                var result =
                    Utilities.ShowAlert(
                        @"El documento para consumidor final excede los RD$ 250,000.00, desea continuar?", "Atencion",
                        MessageBoxIcon.Warning, MessageBoxButtons.YesNo);

                if (result != DialogResult.Yes)
                {
                    StopTransactionExecution(parsedDocument);
                    return;
                }
            }

            switch (taxReceiptInfo.RemainingReceipts)
            {
                case 100:
                case 50:
                case 25:
                case 10:
                    Utilities.ShowAlert(
                        $@"Quedan {taxReceiptInfo.RemainingReceipts} NCFs restantes de '{taxReceiptInfo.ReceiptType}'.",
                        "Atencion", MessageBoxIcon.Warning);
                    break;
                default:
                    if (taxReceiptInfo.RemainingReceipts <= 0 || taxReceiptInfo.NextReceipt.Length == 0 ||
                        !taxReceiptInfo.UniqueReceipt)
                    {
                        Utilities.ShowAlert(
                            $@"No hay NCFs restantes de '{taxReceiptInfo.ReceiptType}'. Contacte con su administrador.",
                            "Error", MessageBoxIcon.Error);
                        StopTransactionExecution(parsedDocument);
                        return;
                    }

                    if (taxReceiptInfo.RemainingReceipts < 10)
                        Utilities.ShowAlert(
                            $@"Quedan {taxReceiptInfo.RemainingReceipts} NCFs restantes de '{taxReceiptInfo.ReceiptType}'. Contacte con su administrador.",
                            "Atencion", MessageBoxIcon.Warning);
                    break;
            }

            Close();
        }

        private void PendingSale_Event(ParsedDocument parsedDocument)
        {
            var hasPriceErrors = parsedDocument.HasPriceErrors();

            if (!hasPriceErrors)
                Close();

            using (Form form = new AuditInvoice(_eventName, hasPriceErrors, false))
            {
                form.ShowDialog();
            }

            InvokeEventInNewProcess("show-price-errors");
        }

        private void AuditQuote_Event(ParsedDocument parsedDocument)
        {
            var notInDollars = parsedDocument.DocumentCurrencyIsNotDollars();

            if (notInDollars)
                using (Form form = new AuditInvoice(_eventName, false, notInDollars))
                {
                    form.ShowDialog();
                }

            Close();
        }

        private void ShowPriceErrors_Event(ParsedDocument parsedDocument)
        {
            var hasPriceErrors = parsedDocument.HasPriceErrors();

            if (hasPriceErrors)
            {
                using (Form form = new PriceErrorList(parsedDocument))
                {
                    form.ShowDialog();
                }

                StopTransactionExecution(parsedDocument);
                return;
            }

            Close();
        }

        private void ShowCurrentRate_Event(ParsedDocument parsedDocument)
        {
            var rate = parsedDocument.GetCurrentRate();

            using (Form form = new CurrentRate(rate))
            {
                form.ShowDialog();
            }

            Close();
        }

        private void UpdateContributors_Event(ParsedDocument parsedDocument)
        {
            using (Form form = new UpdateContributors(parsedDocument))
            {
                form.ShowDialog();
            }

            Close();
        }

        private void AuditDocuments_Event(ParsedDocument parsedDocument)
        {
            var invalidDocuments = parsedDocument.GetInvalidDocuments();

            if (invalidDocuments == null || invalidDocuments.Count == 0)
            {
                Utilities.ShowAlert(
                    "Puede proceder a realizar el cierre Z. Por favor, si realiza alguna factura antes de hacer el Z, tiene que volver a hacer esta validación.");
                Close();
                return;
            }

            Utilities.ShowAlert(
                $@"Hay {invalidDocuments.Count} documentos en el próximo cierre Z con problemas de numero de comprobante, por favor contacte al personal de tecnología para corregirlos antes de presionar el botón Z.",
                "Atención", MessageBoxIcon.Error);

            using (Form form = new AuditDocuments(invalidDocuments))
            {
                form.ShowDialog();
            }

            Close();
        }

        private void StopTransactionExecution(ParsedDocument parsedDocument, bool restoreNcf = false,
            string errorMessage = null)
        {
            switch (parsedDocument.DocumentTypeId)
            {
                case 1:
                    // Do nothing
                    break;
                default:
                    Utilities.UpdateXml(true, errorMessage);
                    if (restoreNcf) parsedDocument.RestoreNcfCounter();
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void SendEsc_Event(int sleepMs = 1500)
        {
            try
            {
#if !DEBUG
                const string processName = "FrontRetail";
#else
                string processName = "notepad";
#endif
                var process = Process.GetProcessesByName(processName).FirstOrDefault();
                var inputSimulator = new InputSimulator();

                if (process == null) return;
                var handle = process.MainWindowHandle;
                Thread.Sleep(sleepMs);
                SetForegroundWindow(handle);

                for (var i = 0; i < 10; i++)
                {
#if !DEBUG
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
#else
                        inputSimulator.Keyboard.TextEntry(i + " ");
#endif
                    Thread.Sleep(800);
                }
#if DEBUG
                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
#endif
            }
            finally
            {
                Close();
            }
        }

        private void OnDocumentSave_Event(ParsedDocument parsedDocument)
        {
            parsedDocument.EnablePriceValidation();

            var erroredDocumentsList = parsedDocument.GetInvalidDocuments();

            if (erroredDocumentsList.Count > 0)
            {
                var isCurrentDocument = erroredDocumentsList.Find(item =>
                    (item.Series == parsedDocument.DocumentSeries ||
                     item.Series == parsedDocument.DocumentFutureSeries) &&
                    item.Number == parsedDocument.DocumentNumber) != null;

                var errorMessage = isCurrentDocument
                    ? @"No se pudo generar un NCF para el documento guardado. Por favor, contacte al personal de tecnología para corregirlos."
                    : @"Existen documentos que fueron generados sin NCF. Utilice el botón 'Auditar Z' para más detalles.";

                StopTransactionExecution(
                    parsedDocument,
                    false,
                    errorMessage
                );
                return;
            }

            var hasPriceErrors = parsedDocument.HasPriceErrors();
            var hasDocumentCurrencyMismatch = parsedDocument.HasDocumentCurrencyMismatch();

            if (hasPriceErrors || hasDocumentCurrencyMismatch)
            {
                using (Form form = new AuditInvoice(_eventName, hasPriceErrors, hasDocumentCurrencyMismatch))
                {
                    form.ShowDialog();
                }

                StopTransactionExecution(
                    parsedDocument,
                    true,
                    "No se pudo guardar el documento debido a errores en los precios."
                );

                InvokeEventInNewProcess("show-price-errors");
                return;
            }

            Close();
        }

        private void OpenCalc_Event()
        {
            using (Form form = new DiscountCalculator())
            {
                form.ShowDialog();
            }

            Close();
        }

        private void InvalidEventName_Event()
        {
            Close();
        }

        private void InvokeEventInNewProcess(string @event)
        {
            if (@event == null)
            {
                Close();
                return;
            }

            var filename = Path.Combine(AppDomain.CurrentDomain.FriendlyName);
            Process.Start(filename, @event);

            Close();
        }

        private void ClearPendingDocuments(ParsedDocument parsedDocument)
        {
            using (Form form = new ClearPendingDocuments(parsedDocument))
            { 
                //var parsedDocument = Utilities.ParseXml();
                form.ShowDialog();
            }

            Close();

        }

        private void SavingLongSerials(ParsedDocument parsedDocument)
        {
            
        }


        private void loadingText_Click(object sender, EventArgs e)
        {

        }
    }
}