using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Plugin_ICGFront.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Plugin_ICGFront
{
    public class ParsedDocument
    {
        private const double PriceMarginAmount = 5;

        public string DatabaseName;
        public string DocumentFutureSeries;
        public string DocumentN;
        public int DocumentNumber;
        public string DocumentSeries;
        public string DocumentType;
        public int DocumentTypeId;
        public string ServerName;
        //public string UserName = "DESKTOP-RC5CHSE"; 
        public string UserName = "ICGPlugin";
        //[Enmanuel]: NUEVO CAMPO DEL CODVENDEDOR
        public string CodVendedor;


        public ParsedDocument(string serverName, string databaseName, string documentType, int documentTypeId,
            string documentSeries, int documentNumber, string documentN, string documentFutureSeries, string codVendedor)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            DocumentType = documentType;
            DocumentTypeId = documentTypeId;
            DocumentSeries = documentSeries;
            DocumentNumber = documentNumber;
            DocumentN = documentN;
            DocumentFutureSeries = documentFutureSeries;
            CodVendedor = codVendedor;
        }


        public SqlConnection GetSqlConnection(string username = null, string password = "masterpwd")
        {
            UserName = username ?? UserName;

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                UserID = UserName,
                Password = password,
                InitialCatalog = DatabaseName
                //,IntegratedSecurity = true
            };
            return new SqlConnection(builder.ConnectionString);
        }

        #region Plugin Borrar Facturas Pendientes
        
        //[Enmanuel]: Comando para verificar existencia de facturas pendientes.
        private SqlCommand PendingDocumendsExistsCmd()
        {
            string query;
            var command = new SqlCommand();

            query = @"SELECT 
	                    CASE 
		                    WHEN (SELECT COUNT(*) FROM ALBVENTACAB WHERE NUMSERIE = '****') > 0 THEN 1 ELSE 0 
                    END EXISTENCIA;";
            command.Connection = GetSqlConnection();
            command.CommandText = query;
            return command;
        }


        //[Enmnauel]: Comando para obtener documentos pendientes
        private SqlCommand GetPendingDocumentCmd() {
            string query;
            var command = new SqlCommand();
 
            query = @"SELECT
	                    A.NUMALBARAN, A.NUMSERIE, B.NOMBRECLIENTE, FORMAT(A.TOTALBRUTO, 'C','es-Do') AS TOTALBRUTO,FORMAT(A.TOTALNETO, 'C','es-Do') AS TOTALNETO, A.CAJA, C.CODVENDEDOR, C.NOMVENDEDOR, 
                         CAST(CONVERT(date, FECHA) AS datetime) + CAST(CAST(Hora AS time(1)) AS datetime) AS FECHA
	                    FROM ALBVENTACAB A
	                    JOIN CLIENTES B	ON A.CODCLIENTE = B.CODCLIENTE
	                    JOIN VENDEDORES C ON A.CODVENDEDOR = C.CODVENDEDOR
						WHERE A.NUMSERIE = '****'
	                    ";
            command.Connection = GetSqlConnection();
            command.CommandText = query;

            return command;
        }

        //[Enmanuel]: Comando para determinar si el vendedor puede eliminar documentos pendientes
        private SqlCommand ShouldBeAbleToDeleteInvoiceCmd()
        {
            var query = @"DECLARE @codVendedor int = @Vendedor;
		                  SELECT 
		                    CASE WHEN EXISTS 
			                    (SELECT 1 FROM VENDEDORES WHERE CODVENDEDOR = @codVendedor AND CODVENDEDOR IN (20,21,22,29,30,32,37,38,40,45,58,59,68)) THEN 'Y' 
			                    ELSE 'N' END AS VendedorValido";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };  

            command.Parameters.AddWithValue("@Vendedor", CodVendedor);
            //command.Parameters.AddWithValue("@Vendedor", SqlDbType.NVarChar).Value = CodVendedor;

            return command;
        }
        //[Enmanuel]: Comando para eliminar documentos pendientes
        private SqlCommand DeletePendingDocumentsCmd()
        {

            const string query = @"DELETE A 
	                                FROM ALBVENTACAB A
	                                JOIN CLIENTES B	ON A.CODCLIENTE = B.CODCLIENTE
	                                JOIN VENDEDORES C ON A.CODVENDEDOR = C.CODVENDEDOR
	                                WHERE A.NUMSERIE = '****'";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };
            return command;
        }
        //[Enmanuel]: Comando para obtener nombre de empleado
        public SqlCommand GetEmployeeNamecmd()
        {
            const string query = @"SELECT V.NOMVENDEDOR FROM VENDEDORES V WHERE V.CODVENDEDOR = @codVendedor";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            var codVendedor = this.CodVendedor;
            command.Parameters.AddWithValue("@codVendedor", SqlDbType.NVarChar).Value = codVendedor;

            return command;
        }

        //[Enmanuel]: Método para validar que el vendedor puede eliminar facuturas pendientes
        public bool CanDeleteInvoices()
        {
            var result = false;
#if !DEBUG
            try
            {
                using (var command = ShouldBeAbleToDeleteInvoiceCmd())
                {
                    command.Connection.Open();
                    var resultado = command.ExecuteScalar().ToString();

                    return resultado == "Y"; //el vendedor está habilitado
                     
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = true;
#endif
            return result;
        }

        //[Enmanuel]: verificar existencia de facturas pendientes.
        public int PendingDocumendsExists()
        {
            int num = 0;

#if !DEBUG
            try
            {
                using (var command = PendingDocumendsExistsCmd())
                {
                    command.Connection.Open();

                    var result = command.ExecuteScalar();

                    if (result != null && result.ToString() != "0")
                        num = 1; 

                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex);
                // ignored
            }
#else
            Nombre = "NULL";
#endif
            return num;
        }


        //[Enmanuel]: Método Consulta Obtener Documentos Pendientes
        public List<InvoicePendingInfo> GetPendingDocuments()
        {

#if !DEBUG
            try
            {
                List<InvoicePendingInfo> queryRows = new List<InvoicePendingInfo>();

                try {

                    using (var command = GetPendingDocumentCmd())
                    {
                        command.Connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            var num_albara_index = reader.GetOrdinal("NUMALBARAN");
                            var num_serie_index = reader.GetOrdinal("NUMSERIE");
                            var nombre_cliente_index = reader.GetOrdinal("NOMBRECLIENTE");
                            var total_bruto_index = reader.GetOrdinal("TOTALBRUTO");
                            var total_neto_index = reader.GetOrdinal("TOTALNETO");
                            var caja_index = reader.GetOrdinal("CAJA");
                            var codigo_vendedor_index = reader.GetOrdinal("CODVENDEDOR");
                            var nombre_vendedor_index = reader.GetOrdinal("NOMVENDEDOR");
                            var fecha_index = reader.GetOrdinal("FECHA");


                            while (reader.Read())
                            {
                                var num_albaran = reader.GetValue(num_albara_index).ToString();
                                var num_serie = reader.GetValue(num_serie_index).ToString();
                                var nombre_cliente = reader.GetValue(nombre_cliente_index).ToString();
                                var total_bruto = reader.GetValue(total_bruto_index).ToString();
                                var total_neto = reader.GetValue(total_neto_index).ToString();
                                var caja = reader.GetValue(caja_index).ToString();
                                var codigo_vendedor = reader.GetValue(codigo_vendedor_index).ToString();
                                var nombre_vendedor = (reader.GetValue(nombre_vendedor_index).ToString());
                                var fecha = reader.GetValue(fecha_index).ToString();


                                InvoicePendingInfo persona = new InvoicePendingInfo
                                {
                                    NUMALBARAN = num_albaran,
                                    NUMSERIE = num_serie,
                                    NOMBRECLIENTE = nombre_cliente,
                                    TOTALBRUTO = total_bruto,
                                    TOTALNETO = total_neto,
                                    CAJA = caja,
                                    CODVENDEDOR = codigo_vendedor,
                                    NOMVENDEDOR = nombre_vendedor,
                                    FECHA = fecha
                                };

                                queryRows.Add(persona);
                            }
                        }
                    }
                }
                
                catch (Exception ex){
                    MessageBox.Show("Error: "+ex);
                } 
                    return queryRows;
                }
           
            catch (Exception)
            {
                // ignored
            }
             
            return null;
#else
           

#endif
        }
        //[Enmanuel]: Método para eliminar documentos pendientes
        public bool DeletePendingDocuments()
        {
            const bool result = false;
#if !DEBUG
            try
            {
                using (var command = DeletePendingDocumentsCmd())
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = false;
#endif
            return result;
        }
        //[Enmanuel]: Método para consultar nombre de empleado
        public string GetEmployeeName()
        {
            string nombre = "No Encontrado";

#if !DEBUG
            try
            {
                using (var command = GetEmployeeNamecmd())
                {
                    command.Connection.Open();

                    object result = command.ExecuteScalar();
                    nombre = result.ToString();
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex);
                // ignored
            }
#else
            Nombre = "NULL";
#endif
            return nombre;

        }

        //[Enmanuel]: comando para saber si la tabla existe
        public SqlCommand TableExistcmd()
        {
            //string table_name = "AuditDeletedPendingInvoices";

            string query = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AuditDeletedPendingInvoices'";
             
            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            //command.Parameters.AddWithValue("@TableName", table_name);
            return command;
        }

        //[Enmanuel]: comando para saber si la tabla existe
        public bool TableExist()
        {
            int contador = 0;
#if !DEBUG
            try
            {
                using (var command = TableExistcmd())
                {
                    command.Connection.Open();

                    object result = command.ExecuteScalar();
                    contador = Convert.ToInt32(result);
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex);
                // ignored
            }
#else
            contador = 0;
#endif
            if (contador >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //[Enmanuel]: comando para crear la tabla:
        public SqlCommand CreateTableAuditDeletedPendingInvoicesCmd()
        {
            string query = @" 
                            USE " + DatabaseName + @";
                            CREATE TABLE AuditDeletedPendingInvoices 
                            (
                                ID int PRIMARY KEY IDENTITY(1,1), 
                                Vendedor_ID int,
                                Nombre_Vendedor nvarchar(100),
                                Accion nvarchar(100),
                                Fecha DATETIME DEFATUL GETDATE()
                            );";


            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            //command.Parameters.AddWithValue("@DataBaseName", DatabaseName);
            return command;
        }

        //[Enmanuel]: Función para crear la tabla AuditDeletedPendingInvoices
        public bool CreateTableAuditDeletedPendingInvoices()
        {
            const bool result = false;
#if !DEBUG
            try
            {
                using (var command = CreateTableAuditDeletedPendingInvoicesCmd())
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    MessageBox.Show("El comando de creación de tabla se ejecutó con éxtio!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: "+ex);
            }
#else
            Result = false;
#endif
            
            return result;
        }

        //[Enmanuel]: Comando para auditar los vendedores que borran facturas pendientes
        public SqlCommand AuditDeletedPendingInvoicescmd(string nombre_trabajador)
        {
            string query = @"INSERT INTO AuditDeletedPendingInvoices (Vendedor_ID, Nombre_Vendedor, Accion) 
                                VALUES (
                                        @CodigoVendedor, 
                                        @NombreVendedor,
                                        'Borró Facturas Pendientes'
                                        )";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@CodigoVendedor", CodVendedor);
            command.Parameters.AddWithValue("@NombreVendedor", nombre_trabajador);
            //command.Parameters.AddWithValue("@fecha", fecha);

            return command;
        }

        //[Enmanuel]: Método para auditar los vendedores que borran facturas pendientes
        public void AuditDeletedPendingInvoices(string nombre_trabajador)
        {
            if (TableExist()) {
#if !DEBUG
                try
                {
                    using (var command = AuditDeletedPendingInvoicescmd(nombre_trabajador))
                    {
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
#else
            Result = false;
#endif
            }
            else
            {
                if (!CreateTableAuditDeletedPendingInvoices())
                {
#if !DEBUG
                    
                    try
                    {
                        using (var command = AuditDeletedPendingInvoicescmd(nombre_trabajador))
                        {
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                            command.Connection.Close();
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
#else
            Result = false;
#endif
                
                }
            }
        }

        //[Enmanuel]: Comando para utilizar la hora del servidor y no de la caja
        public SqlCommand RecuperaHoraServidorCmd()
        {
            string query = "SELECT CONVERT(varchar, GETDATE(), 108) AS CurrentTime;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };
            return command;
        }
        
        //[Enmanuel]: Comando para recuperar día del servidor y no de la caja
        public SqlCommand RecuperaDiaServidorCmd()
        {
            string query = "SELECT DATENAME(WEEKDAY, GETDATE()) AS DIA;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };
            return command;
        }

        //[Enmanuel]: Método para recuperar día del servidor y no de la caja
        public string RecuperaDiaServidor()
        {
            string dia; 
#if !DEBUG
            try
            {
                using (var command = RecuperaDiaServidorCmd())
                {
                    command.Connection.Open();
                    var result = (string)command.ExecuteScalar();
                    command.Connection.Close();

                    dia = result.ToString();
                    
                    return dia;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: "+ex);
                return "error";
            }
#endif
            return null;
        }

        #endregion



        #region Plugin Cambiar Tasa Nota de Crédito

        public SqlCommand GetInvoiceRateCmd()
        {
            const string query = @"
                                SELECT TOP 1 FACTORMONEDA
                                FROM ALBVENTACAB
                                WHERE CODMONEDA = 2
                                  AND NUMSERIE = @Serie
                                  AND NUMFAC = @Numero;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            //command.Parameters.AddWithValue("@Serie", serie);
            //command.Parameters.AddWithValue("@Numero", numero);
            command.Parameters.AddWithValue("@Serie", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@Numero", SqlDbType.Int).Value = DocumentNumber;

            return command;
        }

        public decimal? GetInvoiceRate()
        {
#if !DEBUG
            using (var command = GetInvoiceRateCmd())
            {
                command.Connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToDecimal(result);
            }
#else
    return 58.50m;
#endif
        }

        public SqlCommand GetCurrencyRateCmd()
        {
            const string query = @"
                        SELECT TASA 
                        FROM MONEDAS 
                        WHERE CODMONEDA = 2;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            return command;
        }

        public decimal GetCurrencyRate()
        {
#if !DEBUG
            using (var command = GetCurrencyRateCmd())
            {
                command.Connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return 0;

                return Convert.ToDecimal(result);
            }
#else
    return 60.00m;
#endif
        }

        public SqlCommand GetTodayQuotationRateCmd()
        {
            const string query = @"
                        SELECT TOP 1 COTIZACION
                        FROM COTIZACIONES
                        WHERE CAST(FECHA AS DATE) = CAST(GETDATE() AS DATE);";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            return command;
        }

        public decimal GetTodayQuotationRate()
        {
#if !DEBUG
            using (var command = GetTodayQuotationRateCmd())
            {
                command.Connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return 0;

                return Convert.ToDecimal(result);
            }
#else
    return 60.00m;
#endif
        }

        public bool ChangeRateForCreditMemo(decimal nuevaTasa)
        {
#if !DEBUG
            using (SqlConnection connection = GetSqlConnection())
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateMonedas = @"
                                        UPDATE MONEDAS 
                                        SET COTDEF = @NuevaTasa
                                        WHERE CODMONEDA = 2;";

                        using (SqlCommand cmdMonedas = new SqlCommand(updateMonedas, connection, transaction))
                        {
                            cmdMonedas.Parameters.AddWithValue("@NuevaTasa", nuevaTasa);
                            cmdMonedas.ExecuteNonQuery();
                        }

                        string updateCotizaciones = @"
                                        UPDATE COTIZACIONES
                                        SET COTIZACION = @NuevaTasa
                                        WHERE CAST(FECHA AS DATE) = CAST(GETDATE() AS DATE);";

                        using (SqlCommand cmdCotizaciones = new SqlCommand(updateCotizaciones, connection, transaction))
                        {
                            cmdCotizaciones.Parameters.AddWithValue("@NuevaTasa", nuevaTasa);
                            cmdCotizaciones.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
#else
    return true;
#endif
        }

        public DateTime GetServerDateTime()
        {
#if !DEBUG
            using (var command = RecuperaHoraServidorCompletaCmd())
            {
                command.Connection.Open();

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return DateTime.Now;

                return Convert.ToDateTime(result);
            }
#else
    return DateTime.Now;
#endif
        }

        public SqlCommand RecuperaHoraServidorCompletaCmd()
        {
            const string query = "SELECT GETDATE();";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            return command;
        }

        #endregion




        #region Proyecto Seriales

        private SqlCommand Cantidad_Kits_FacturaCmd()
        {
            string query;
            var command = new SqlCommand();

            query = @"SELECT COUNT(B.NUMALBARAN)
                        FROM ALBVENTACAB A
                        JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN
                        JOIN ARTICULOS C ON B.CODARTICULO = C.CODARTICULO
                        JOIN KITS D ON C.CODARTICULO = D.CODARTICULO
                        WHERE A.NUMSERIE = @SERIE AND A.NUMALBARAN = @ALBARAN;";
            command.Connection = GetSqlConnection();
            command.CommandText = query;

            command.Parameters.AddWithValue("@SERIE", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@ALBARAN", SqlDbType.Int).Value = DocumentNumber;

            return command;
        }
        private SqlCommand Obtener_Articulos_CantidadCmd()
        {
            string query;
            var command = new SqlCommand();

            query = @"SELECT D.CODARTICULO AS CODIGOARTICULO, SUM(D.UNIDADES) AS CANTIDAD_ARTICULOS 
                        FROM ALBVENTACAB A
                        JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN
                        JOIN ARTICULOS C ON B.CODARTICULO = C.CODARTICULO
                        JOIN KITS D ON C.CODARTICULO = D.CODARTICULO
                        WHERE A.NUMSERIE = @SERIE AND A.NUMALBARAN = @ALBARAN
                        GROUP BY D.CODARTICULO;";
            command.Connection = GetSqlConnection();
            command.CommandText = query;

            command.Parameters.AddWithValue("@SERIE", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@ALBARAN", SqlDbType.Int).Value = DocumentNumber;
             
            return command;
        }
        private SqlCommand Obtener_Cantidad_Maxima_Articulos_kitCmd()
        {
            string query;
            var command = new SqlCommand();

            query = @"SELECT MAX(SUMA)
                        FROM (
                            SELECT SUM(D.UNIDADES) AS SUMA 
                            FROM ALBVENTACAB A
                            JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN
                            JOIN ARTICULOS C ON B.CODARTICULO = C.CODARTICULO
                            JOIN KITS D ON C.CODARTICULO = D.CODARTICULO
                            WHERE A.NUMSERIE = @SERIE AND A.NUMALBARAN = @ALBARAN
                            GROUP BY D.CODARTICULO
                        ) AS X;";
            command.Connection = GetSqlConnection();
            command.CommandText = query;

            command.Parameters.AddWithValue("@SERIE", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@ALBARAN", SqlDbType.Int).Value = DocumentNumber;

            return command;
        }
        private SqlCommand InsertarSerialesCmd(List<String> sub_cadenas)
        {
            string query;
            var command = new SqlCommand();


            // Crear el comando dinámicamente según el número de subcadenas
            string columnas = string.Join(",", sub_cadenas.Select((_, i) => $"COLUMNA{i + 1}"));
            string valores = string.Join(",", sub_cadenas.Select((_, i) => $"@col{i + 1}"));
            string aux_query = $"INSERT INTO ALBVENTACAMPOSLIBRES ({columnas}) VALUES ({valores})";//   !!IMPORTANTE!! DEBES CAMBIAR ESTE QUERY POR LOS NOMRBES CORRECTOS

            query = aux_query;

            // Agregar los parámetros
            for (int i = 0; i < sub_cadenas.Count; i++)
            {
                command.Parameters.AddWithValue($"@col{i + 1}", sub_cadenas[i]);
            }

            command.Connection = GetSqlConnection();
            command.CommandText = query;
            return command;
        }

        
        private SqlCommand RecuperarDatosCmd()
        {
            string query;
            var command = new SqlCommand();

            query = @"SELECT columna1, columna2, columna3, columna4 
                        FROM ALBVENTACAMPOSLIBRES 
                        WHERE id = 1";
            command.Connection = GetSqlConnection();
            command.CommandText = query;



            return command;
        }
        
        public String RecuperarDatos()
        {

            string datos = string.Empty;
#if !DEBUG
            try
            {
                using (var command = RecuperarDatosCmd())
                {
                    command.Connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            datos = $"{reader["columna1"]}, {reader["columna2"]}, {reader["columna3"]}, {reader["columna4"]}";
                        }
                        reader.Close();
                        command.Connection.Close();
                        return datos;

                    }

                    return datos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                return datos;
            }
#endif
            return null;
        }

        //INSERTAR SERIALES
        public bool InsertarSeriales(List<String> sub_cadenas)
        {
            const bool result = false;
#if !DEBUG
            try
            {
                using (var command = InsertarSerialesCmd(sub_cadenas))
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                return false;
            }
#else
            Result = false;
#endif
            return result;
        }

        





        //CUÁNTOS KITS HAY EN LA FACTURA:
        public int Cantidad_Kits_Factura()
        {
#if !DEBUG
            try
            {
                using (var command = Cantidad_Kits_FacturaCmd())
                {
                    command.Connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar().ToString());

                    return cantidad;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                return -1;
            }
#endif
        }
        //CUÁL ES EL KIT CON MÁS SERIALES:
        public int obtener_cantidad_maxima_articulos_kit()
        {
#if !DEBUG
            try
            {
                using (var command = Obtener_Cantidad_Maxima_Articulos_kitCmd())
                {
                    command.Connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar().ToString());

                    return cantidad;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                return -1;
            }
#endif
        }
        //CUÁLES SON LOS ARTÍCULOS/KITS Y SUS CANTIDADES EN LA FACTURA?
        public List<Articulo_Cantidad> Obtener_Articulos_Cantidad()
        {
            List<Articulo_Cantidad> list_articulos = new List<Articulo_Cantidad>();
#if !DEBUG
            try
            {
                using (var command = Obtener_Articulos_CantidadCmd())
                {
                    command.Connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var cod_art_index = reader.GetOrdinal("CODIGOARTICULO");//CAMBIAR POR NOMBRE COLUMNA REAL
                        var cantidad_index = reader.GetOrdinal("CANTIDAD_ARTICULOS");//CAMBIAR POR NOMBRE COLUMNA REAL

                        while (reader.Read())
                        {
                            var cod_art = reader.GetValue(cod_art_index).ToString();
                            var cantidad = Convert.ToInt32(reader.GetValue(cantidad_index).ToString());

                            Articulo_Cantidad articulo = new Articulo_Cantidad
                            {
                                CODARTICULO = cod_art,
                                CANTIDAD = cantidad
                            };
                            list_articulos.Add(articulo);
                        }
                        reader.Close();
                        command.Connection.Close();
                        return list_articulos;

                    }

                    return list_articulos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                return list_articulos;
            }
#endif
            return null;
        }



        #endregion
























        //        public void RecuperaHoraServidor(object sender, EventArgs e)
        //        {

        //#if !DEBUG
        //            try
        //            {
        //                using (var command = RecuperaHoraServidorCmd())
        //                {
        //                    command.Connection.Open();
        //                    SqlDataReader reader = command.ExecuteReader();
        //                    command.Connection.Close();

        //                    if (reader.Read())
        //                    {
        //                        string currentDateTime = reader.GetString(0);
        //                        return currentDateTime;
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Algo salió mal recuperando la hora del servidor: " + ex);
        //            }
        //#endif

        //            return "NULL";
        //        }





        private SqlCommand GetInvoiceErroredPricesCmd()
        {
            string query;
            var command = new SqlCommand();

            switch (DocumentTypeId)
            {
                case 3:
                    query = $@"
                    SELECT
                        CASE WHEN VALIDAR_PRECIOS = 1 THEN COUNT(*) ELSE 0 END LINEASERRONEAS
                    FROM (
	                    SELECT 
		                    X.*,
                            CAST(CASE WHEN X.PRECIO > 100 THEN {PriceMarginAmount} ELSE 1 END AS DECIMAL) / X.FACTORMONEDA MARGEN,
		                    (X.PRECIOCONIVA1 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG1, 
		                    (X.PRECIOCONIVA2 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG2
	                    FROM (
		                    SELECT 
			                    C.NUMSERIE, 
			                    B.NUMFAC, 
			                    C.NUMLIN, 
			                    C.REFERENCIA, 
			                    C.DESCRIPCION, 
			                    C.UNIDADESTOTAL, 
			                    B.CODMONEDA,
                                B.FACTORMONEDA,
			                    F.COTDEF,
			                    ISNULL(E.COTIZACION, 1) COTIZACION1,
			                    ISNULL(I.COTDEF, 1) COTIZACION2,
			                    C.DTO,
                                C.IVA,
			                    (SELECT TOP 1 IVA / 100 FROM IMPUESTOS WHERE DESCRIPCION = 'ITBIS') + 1 FACTORIVA,
			                    (CASE WHEN B.CODMONEDA = 2 THEN C.PRECIO * F.COTDEF ELSE C.PRECIO / F.COTDEF END) * ((100 - C.DTO) / 100) * (1 + (C.IVA / 100)) PRECIO, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(I.COTDEF, 1) ELSE G.PNETO / ISNULL(I.COTDEF, 1) END) PRECIOCONIVA1, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(E.COTIZACION, 1) ELSE G.PNETO / ISNULL(E.COTIZACION, 1) END) PRECIOCONIVA2, 
			                    D.VALIDAR_PRECIOS 
	                    FROM ALBVENTACAB B 
	                    JOIN ALBVENTALIN C ON B.NUMSERIE = C.NUMSERIE AND B.NUMALBARAN = C.NUMALBARAN AND B.N = C.N 
	                    JOIN ICGFRONT_PLUGIN_CONFIG D ON B.CAJA = D.CAJA COLLATE Latin1_General_CS_AI
	                    JOIN MONEDAS F ON B.CODMONEDA = F.CODMONEDA
	                    JOIN PRECIOSVENTA G ON G.CODARTICULO = C.CODARTICULO AND G.IDTARIFAV = C.CODTARIFA AND G.TALLA = '.' AND G.COLOR = '.' AND G.PNETO > 0
	                    JOIN MONEDAS I ON I.CODMONEDA = 2
	                    LEFT JOIN (SELECT D.CODMONEDA, D.COTIZACION FROM COTIZACIONES D ORDER BY D.FECHA DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) E ON G.CODMONEDA = E.CODMONEDA
	                    WHERE B.NUMSERIE = @DocumentSeries AND B.NUMALBARAN = @DocumentNumber AND B.TIPODOC = @DocumentTypeId AND C.UNIDADESTOTAL > 0 AND LEN(C.REFERENCIA) > 0) X
                    ) Y 
                    WHERE (Y.PRECIO < (Y.PRECIOSUG1 - Y.MARGEN) AND Y.PRECIO < (Y.PRECIOSUG2 - Y.MARGEN)) OR (Y.PRECIO > (Y.PRECIOSUG1 + Y.MARGEN) AND Y.PRECIO > (Y.PRECIOSUG2 + Y.MARGEN))
                    GROUP BY Y.VALIDAR_PRECIOS;";
                    break;

                default:
                    query = $@"
                    SELECT
                        CASE WHEN VALIDAR_PRECIOS = 1 THEN COUNT(*) ELSE 0 END LINEASERRONEAS
                    FROM (
	                    SELECT 
		                    X.*,
                            CAST(CASE WHEN X.PRECIO > 100 THEN {PriceMarginAmount} ELSE 1 END AS DECIMAL) / X.FACTORMONEDA MARGEN,
                            (X.PRECIOCONIVA1 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG1, 
		                    (X.PRECIOCONIVA2 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG2
	                    FROM (
		                    SELECT 
			                    C.NUMSERIE, 
			                    B.NUMFAC, 
			                    C.NUMLIN, 
			                    C.REFERENCIA, 
			                    C.DESCRIPCION, 
			                    C.UNIDADESTOTAL, 
			                    B.CODMONEDA,
                                B.FACTORMONEDA,
			                    F.COTDEF,
			                    ISNULL(E.COTIZACION, 1) COTIZACION1,
			                    ISNULL(I.COTDEF, 1) COTIZACION2,
			                    C.DTO,
                                C.IVA,
			                    (SELECT TOP 1 IVA / 100 FROM IMPUESTOS WHERE DESCRIPCION = 'ITBIS') + 1 FACTORIVA,
			                    (CASE WHEN B.CODMONEDA = 2 THEN C.PRECIO * F.COTDEF ELSE C.PRECIO / F.COTDEF END) * ((100 - C.DTO) / 100) * (1 + (C.IVA / 100)) PRECIO, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(I.COTDEF, 1) ELSE G.PNETO / ISNULL(I.COTDEF, 1) END) PRECIOCONIVA1, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(E.COTIZACION, 1) ELSE G.PNETO / ISNULL(E.COTIZACION, 1) END) PRECIOCONIVA2, 
			                    D.VALIDAR_PRECIOS 
	                    FROM ALBVENTACAB B 
	                    JOIN ALBVENTALIN C ON B.NUMSERIE = C.NUMSERIE AND B.NUMALBARAN = C.NUMALBARAN AND B.N = C.N 
	                    JOIN ICGFRONT_PLUGIN_CONFIG D ON B.CAJA = D.CAJA COLLATE Latin1_General_CS_AI
	                    JOIN MONEDAS F ON B.CODMONEDA = F.CODMONEDA
	                    JOIN PRECIOSVENTA G ON G.CODARTICULO = C.CODARTICULO AND G.IDTARIFAV = C.CODTARIFA AND G.TALLA = '.' AND G.COLOR = '.' AND G.PNETO > 0
	                    JOIN MONEDAS I ON I.CODMONEDA = 2
	                    LEFT JOIN (SELECT D.CODMONEDA, D.COTIZACION FROM COTIZACIONES D ORDER BY D.FECHA DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) E ON G.CODMONEDA = E.CODMONEDA
	                    WHERE B.NUMSERIEFAC = @DocumentSeries AND B.NUMFAC = @DocumentNumber AND B.TIPODOC = @DocumentTypeId AND C.UNIDADESTOTAL > 0 AND LEN(C.REFERENCIA) > 0) X
                    ) Y 
                    WHERE (Y.PRECIO < (Y.PRECIOSUG1 - Y.MARGEN) AND Y.PRECIO < (Y.PRECIOSUG2 - Y.MARGEN)) OR (Y.PRECIO > (Y.PRECIOSUG1 + Y.MARGEN) AND Y.PRECIO > (Y.PRECIOSUG2 + Y.MARGEN))
                    GROUP BY Y.VALIDAR_PRECIOS;";
                    break;
            }

            command.Connection = GetSqlConnection();
            command.CommandText = query;

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand GetInvoiceErroredPricesDetailedCmd()
        {
            string query;
            var command = new SqlCommand();

            switch (DocumentTypeId)
            {
                case 3:
                    query = $@"
                    SELECT
                        Y.REFERENCIA, Y.DESCRIPCION, Y.PRECIO, Y.PRECIOSUG1 PRECIOSUG
                    FROM (
	                    SELECT 
		                    X.*,
                            CAST(CASE WHEN X.PRECIO > 100 THEN {PriceMarginAmount} ELSE 1 END AS DECIMAL) / X.FACTORMONEDA MARGEN,
		                    (X.PRECIOCONIVA1 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG1, 
		                    (X.PRECIOCONIVA2 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG2
	                    FROM (
		                    SELECT 
			                    C.NUMSERIE, 
			                    B.NUMFAC, 
			                    C.NUMLIN, 
			                    C.REFERENCIA, 
			                    C.DESCRIPCION, 
			                    C.UNIDADESTOTAL, 
			                    B.CODMONEDA,
                                B.FACTORMONEDA,
			                    F.COTDEF,
			                    ISNULL(E.COTIZACION, 1) COTIZACION1,
			                    ISNULL(I.COTDEF, 1) COTIZACION2,
			                    C.DTO,
                                C.IVA,
			                    (SELECT TOP 1 IVA / 100 FROM IMPUESTOS WHERE DESCRIPCION = 'ITBIS') + 1 FACTORIVA,
			                    ((CASE WHEN B.CODMONEDA = 2 THEN C.PRECIO * F.COTDEF ELSE C.PRECIO / F.COTDEF END) * ((100 - C.DTO) / 100) * (1 + (C.IVA / 100))) / B.FACTORMONEDA PRECIO, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(I.COTDEF, 1) ELSE G.PNETO / ISNULL(I.COTDEF, 1) END) / B.FACTORMONEDA PRECIOCONIVA1, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(E.COTIZACION, 1) ELSE G.PNETO / ISNULL(E.COTIZACION, 1) END) / B.FACTORMONEDA PRECIOCONIVA2, 
			                    D.VALIDAR_PRECIOS 
	                    FROM ALBVENTACAB B 
	                    JOIN ALBVENTALIN C ON B.NUMSERIE = C.NUMSERIE AND B.NUMALBARAN = C.NUMALBARAN AND B.N = C.N 
	                    JOIN ICGFRONT_PLUGIN_CONFIG D ON B.CAJA = D.CAJA COLLATE Latin1_General_CS_AI
	                    JOIN MONEDAS F ON B.CODMONEDA = F.CODMONEDA
	                    JOIN PRECIOSVENTA G ON G.CODARTICULO = C.CODARTICULO AND G.IDTARIFAV = C.CODTARIFA AND G.TALLA = '.' AND G.COLOR = '.' AND G.PNETO > 0
	                    JOIN MONEDAS I ON I.CODMONEDA = 2
	                    LEFT JOIN (SELECT D.CODMONEDA, D.COTIZACION FROM COTIZACIONES D ORDER BY D.FECHA DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) E ON G.CODMONEDA = E.CODMONEDA
	                    WHERE B.NUMSERIE = @DocumentSeries AND B.NUMALBARAN = @DocumentNumber AND B.TIPODOC = @DocumentTypeId AND C.UNIDADESTOTAL > 0 AND LEN(C.REFERENCIA) > 0) X
                    ) Y 
                    WHERE (Y.PRECIO < (Y.PRECIOSUG1 - Y.MARGEN) AND Y.PRECIO < (Y.PRECIOSUG2 - Y.MARGEN)) OR (Y.PRECIO > (Y.PRECIOSUG1 + Y.MARGEN) AND Y.PRECIO > (Y.PRECIOSUG2 + Y.MARGEN));";
                    break;
                default:
                    query = $@"
                    SELECT
                        Y.REFERENCIA, Y.DESCRIPCION, Y.PRECIO, Y.PRECIOSUG1 PRECIOSUG
                    FROM (
	                    SELECT 
		                    X.*,
                            CAST(CASE WHEN X.PRECIO > 100 THEN {PriceMarginAmount} ELSE 1 END AS DECIMAL) / X.FACTORMONEDA MARGEN,
                            (X.PRECIOCONIVA1 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG1, 
		                    (X.PRECIOCONIVA2 * ((100 - X.DTO) / 100)) / (CASE WHEN X.IVA = 0 THEN X.FACTORIVA ELSE 1 END) PRECIOSUG2
	                    FROM (
		                    SELECT 
			                    C.NUMSERIE, 
			                    B.NUMFAC, 
			                    C.NUMLIN, 
			                    C.REFERENCIA, 
			                    C.DESCRIPCION, 
			                    C.UNIDADESTOTAL, 
			                    B.CODMONEDA,
                                B.FACTORMONEDA,
			                    F.COTDEF,
			                    ISNULL(E.COTIZACION, 1) COTIZACION1,
			                    ISNULL(I.COTDEF, 1) COTIZACION2,
			                    C.DTO,
                                C.IVA,
			                    (SELECT TOP 1 IVA / 100 FROM IMPUESTOS WHERE DESCRIPCION = 'ITBIS') + 1 FACTORIVA,
			                    ((CASE WHEN B.CODMONEDA = 2 THEN C.PRECIO * F.COTDEF ELSE C.PRECIO / F.COTDEF END) * ((100 - C.DTO) / 100) * (1 + (C.IVA / 100))) / B.FACTORMONEDA PRECIO, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(I.COTDEF, 1) ELSE G.PNETO / ISNULL(I.COTDEF, 1) END) / B.FACTORMONEDA PRECIOCONIVA1, 
			                    (CASE WHEN G.CODMONEDA = 2 THEN G.PNETO * ISNULL(E.COTIZACION, 1) ELSE G.PNETO / ISNULL(E.COTIZACION, 1) END) / B.FACTORMONEDA PRECIOCONIVA2, 
			                    D.VALIDAR_PRECIOS 
	                    FROM ALBVENTACAB B 
	                    JOIN ALBVENTALIN C ON B.NUMSERIE = C.NUMSERIE AND B.NUMALBARAN = C.NUMALBARAN AND B.N = C.N 
	                    JOIN ICGFRONT_PLUGIN_CONFIG D ON B.CAJA = D.CAJA COLLATE Latin1_General_CS_AI
	                    JOIN MONEDAS F ON B.CODMONEDA = F.CODMONEDA
	                    JOIN PRECIOSVENTA G ON G.CODARTICULO = C.CODARTICULO AND G.IDTARIFAV = C.CODTARIFA AND G.TALLA = '.' AND G.COLOR = '.' AND G.PNETO > 0
	                    JOIN MONEDAS I ON I.CODMONEDA = 2
	                    LEFT JOIN (SELECT D.CODMONEDA, D.COTIZACION FROM COTIZACIONES D ORDER BY D.FECHA DESC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY) E ON G.CODMONEDA = E.CODMONEDA
	                    WHERE B.NUMSERIEFAC = @DocumentSeries AND B.NUMFAC = @DocumentNumber AND B.TIPODOC = @DocumentTypeId AND C.UNIDADESTOTAL > 0 AND LEN(C.REFERENCIA) > 0) X
                    ) Y 
                    WHERE (Y.PRECIO < (Y.PRECIOSUG1 - Y.MARGEN) AND Y.PRECIO < (Y.PRECIOSUG2 - Y.MARGEN)) OR (Y.PRECIO > (Y.PRECIOSUG1 + Y.MARGEN) AND Y.PRECIO > (Y.PRECIOSUG2 + Y.MARGEN));";
                    break;
            }

            command.Connection = GetSqlConnection();
            command.CommandText = query;

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand GetInvoiceWithNegativeDiscountsCmd()
        {
            const string query = @"
            SELECT
                CASE WHEN D.VALIDAR_PRECIOS = 1 THEN COUNT(*) ELSE 0 END LINEASERRONEAS
            FROM ALBVENTACAB B 
            JOIN ALBVENTALIN C ON B.NUMSERIE = C.NUMSERIE AND B.NUMALBARAN = C.NUMALBARAN AND B.N = C.N 
            JOIN ICGFRONT_PLUGIN_CONFIG D ON B.CAJA = D.CAJA COLLATE Latin1_General_CS_AI
            WHERE B.NUMSERIEFAC = @DocumentSeries AND B.NUMFAC = @DocumentNumber AND B.TIPODOC = @DocumentTypeId AND C.UNIDADESTOTAL > 0 AND LEN(C.REFERENCIA) > 0 AND C.DTO < 0 
            GROUP BY D.VALIDAR_PRECIOS;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand GetInvoiceErroredCurrencyCmd()
        {
            const string query = @"
            SELECT
                CASE WHEN CHARINDEX(B.INICIALES, C.MONEDAS_PERMITIDAS COLLATE Latin1_General_CS_AI) > 0 THEN 'Y' ELSE 'N' END ESVALIDA
            FROM ALBVENTACAB A
            JOIN MONEDAS B ON A.CODMONEDA = B.CODMONEDA
            JOIN ICGFRONT_PLUGIN_CONFIG C ON A.CAJA = C.CAJA COLLATE Latin1_General_CS_AI
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand GetQuoteCurrencyCmd()
        {
            const string query = @"
            SELECT 
	            CASE WHEN UPPER(B.DESCRIPCION) LIKE '%DOLARES%' THEN 'Y' ELSE 'N' END COTIZACION_EN_DOLARES 
            FROM PEDVENTACAB A 
            JOIN MONEDAS B ON A.CODMONEDA = B.CODMONEDA 
            WHERE A.NUMSERIE = @DocumentSeries AND A.NUMPEDIDO = @DocumentNumber AND A.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand UpdateValidatePriceCmd()
        {
            const string query = @"UPDATE A SET A.VALIDAR_PRECIOS = 1 FROM ICGFRONT_PLUGIN_CONFIG A WHERE A.CAJA = @CashRegister";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            var cashRegister = DocumentSeries.Substring(0, 3);
            command.Parameters.AddWithValue("@CashRegister", SqlDbType.NVarChar).Value = cashRegister;

            return command;
        }

        private SqlCommand ShouldBeAbleToCreateInvoiceCmd()
        {
            var query = @"
            SELECT
                CASE WHEN C.PERMITIR_FACTURAR = 1 THEN 'Y' ELSE 'N' END PERMITIR_FACTURAR
            FROM ALBVENTACAB A
            LEFT JOIN ICGFRONT_PLUGIN_CONFIG C ON A.CAJA = C.CAJA COLLATE Latin1_General_CS_AI
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.Add("@DocumentSeries", SqlDbType.NVarChar, 50).Value = DocumentSeries;
            command.Parameters.Add("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.Add("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            //command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            //command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            //command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand ShouldBeAbleToGenerateNcfCmd()
        {
            const string query = @"
            SELECT
                CASE WHEN C.GENERAR_NCF_TRIGGER = 1 THEN 'Y' ELSE 'N' END GENERAR_NCF_TRIGGER
            FROM ALBVENTACAB A
            LEFT JOIN ICGFRONT_PLUGIN_CONFIG C ON A.CAJA = C.CAJA COLLATE Latin1_General_CS_AI
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public bool HasPriceErrors()
        {
            var result = false;
#if !DEBUG
            try
            {
                using (var commandForPrices = GetInvoiceErroredPricesCmd())
                using (var commandForDiscounts = GetInvoiceWithNegativeDiscountsCmd())
                {
                    commandForPrices.Connection.Open();
                    commandForDiscounts.Connection.Open();

                    var countForPrices = commandForPrices.ExecuteScalar();
                    var countForDiscounts = commandForDiscounts.ExecuteScalar();

                    var count = 0;

                    if (countForPrices != null) count += (int)countForPrices;

                    if (countForDiscounts != null) count += (int)countForDiscounts;

                    result = count > 0;

                    commandForPrices.Connection.Close();
                    commandForDiscounts.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = false;
#endif
            return result;
        }

        public bool HasDocumentCurrencyMismatch()
        {
            var result = false;
#if !DEBUG
            try
            {
                using (var command = GetInvoiceErroredCurrencyCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<string>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndex = reader.GetOrdinal("ESVALIDA");

                        while (reader.Read()) queryRows.Add(reader.GetValue(columnIndex).ToString());
                    }

                    result = queryRows.Count > 0 && queryRows.First() == "N";

                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = false;
#endif
            return result;
        } 

        public bool DocumentCurrencyIsNotDollars()
        {
            var result = false;
#if !DEBUG
            try
            {
                using (var command = GetQuoteCurrencyCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<string>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndex = reader.GetOrdinal("COTIZACION_EN_DOLARES");

                        while (reader.Read()) queryRows.Add(reader.GetValue(columnIndex).ToString());
                    }

                    result = queryRows.Count > 0 && queryRows.First() == "N";

                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = false;
#endif
            return result;
        }

        public bool EnablePriceValidation()
        {
            const bool result = false;
#if !DEBUG
            try
            {
                using (var command = UpdateValidatePriceCmd())
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = false;
#endif
            return result;
        }

        public bool CanCreateInvoices()
        {
            var result = false;

#if !DEBUG
            try
            {
                using (var command = ShouldBeAbleToCreateInvoiceCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<string>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndex = reader.GetOrdinal("PERMITIR_FACTURAR");

                        while (reader.Read())
                            queryRows.Add(reader.GetString(columnIndex));
                    }

                    MessageBox.Show(
                        $"Cantidad de filas: {queryRows.Count}. Puede facturar: {queryRows.FirstOrDefault()}");

                    result = queryRows.Count > 0 && queryRows[0] == "Y";
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    $"ERROR SQL\n\n" +
                    $"Número: {ex.Number}\n" +
                    $"Mensaje: {ex.Message}\n" +
                    $"Procedimiento: {ex.Procedure}\n" +
                    $"Línea: {ex.LineNumber}",
                    "Error SQL Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"ERROR GENERAL\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
#else
    result = true;
#endif

            return result;
        }


        /*public bool CanCreateInvoices()
        {
            var result = false;
#if !DEBUG
            try
            {
                using (var command = ShouldBeAbleToCreateInvoiceCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<string>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndex = reader.GetOrdinal("PERMITIR_FACTURAR");

                        while (reader.Read()) queryRows.Add(reader.GetValue(columnIndex).ToString());
                    }

                    MessageBox.Show($"Cantidad de filas: {queryRows.Count}. Puede facturar: {queryRows.First()}");
                    result = queryRows.Count > 0 && queryRows.First() == "Y";

                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error en el try-catch");
                // ignored
            }
#else
            Result = true;
#endif
            return result;
        }*/

        public bool CanCreateInvoicesForFp()
        {
            var result = false;
#if !DEBUG
            if (!DocumentSeries.Contains("1") || !DocumentFutureSeries.Contains("1")) return false;

            try
            {
                using (var command = ShouldBeAbleToGenerateNcfCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<string>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndex = reader.GetOrdinal("GENERAR_NCF_TRIGGER");

                        while (reader.Read()) queryRows.Add(reader.GetValue(columnIndex).ToString());
                    }

                    result = queryRows.Count > 0 && queryRows.First() == "Y";

                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = true;
#endif
            return result;
        }

        private SqlCommand GetTaxReceiptInfoCmd()
        {
            const string query = @"
                     SELECT 
    F.*,
    CASE WHEN G.NUMSERIE IS NULL THEN 'Y' ELSE 'N' END COMPROBANTE_UNICO
FROM (SELECT  
    CASE WHEN W.ACTIVO = 0 THEN 'Y' ELSE 'N' END ACTIVO, 	          
    CASE WHEN W.FECHAVENCIMIENTO >= GETDATE() THEN 'Y' ELSE 'N' END NO_VENCIDO,	           
    ISNULL(W.NUMEROFINAL - W.CONTADOR, 0) COMPROBANTES_RESTANTES,	           
    CONCAT(CONCAT('E', RIGHT(W.NUMRESOL, 2)), RIGHT(CONCAT(REPLICATE('0', 10), W.CONTADOR + 1), 10)) COMPROBANTE_SIGUIENTE,  
    ISNULL(Z.DESCRIPCION, 'NOTA DE CREDITO') TIPO_COMPROBANTE,
    X.TIPOCLIENTE TIPO_CLIENTE,
    X.TOTAL_NETO,
    X.NUMERO_COMPROBANTE
FROM (SELECT 
    CASE WHEN X.TOTAL_NETO < 0 THEN '34' ELSE X.TIPOCLIENTE END TIPOCLIENTE,
    CONCAT(X.CAJA, '-', CASE WHEN X.TOTAL_NETO < 0 THEN '34' ELSE X.TIPOCLIENTE END) NUMRESOL,
    X.TOTAL_NETO,
    X.NUMERO_COMPROBANTE
FROM (SELECT	
          RIGHT(CONCAT('00', CASE WHEN ISNULL(Z.CLIF_TIPOCLIENTE, '0') <> '0' 
					                    THEN (CASE 
							                     WHEN Z.CLIF_TIPOCLIENTE = 1 THEN 31 
							                     WHEN Z.CLIF_TIPOCLIENTE = 2 THEN 32 
							                     WHEN Z.CLIF_TIPOCLIENTE = 14 THEN 44 
							                     WHEN Z.CLIF_TIPOCLIENTE = 15 THEN 45 
							                     WHEN Z.CLIF_TIPOCLIENTE = 16 THEN 46 									 
						                      END)
					                    ELSE 
						                    (CASE 
							                     WHEN Y.TIPO = 1 THEN 31 
							                     WHEN Y.TIPO = 2 THEN 32 
							                     WHEN Y.TIPO = 14 THEN 44 
							                     WHEN Y.TIPO = 15 THEN 45 
							                     WHEN Y.TIPO = 16 THEN 46 
							                     ELSE 2
						                      END) 
                       END), 2)  
                   TIPOCLIENTE,
    
    X.CAJA,	            
    X.TOTALNETO * X.FACTORMONEDA TOTAL_NETO,
    X.NUMSERIEFAC,	            
    X.NUMFAC,
    X.N,	            
    X.TIPODOC,
    X.CODCLIENTE,
   CONCAT(CONCAT('E', RIGHT(W.SERIEFISCAL2, 2)), RIGHT(CONCAT(REPLICATE('0', 10), W.NUMEROFISCAL), 10)) NUMERO_COMPROBANTE  
FROM ALBVENTACAB X      
JOIN CLIENTES Y ON X.CODCLIENTE = Y.CODCLIENTE
LEFT JOIN FACTURASVENTASERIESRESOL W ON X.NUMSERIEFAC = W.NUMSERIE AND X.NUMFAC = W.NUMFACTURA AND X.N = W.N
LEFT JOIN FACTURASVENTACAMPOSLIBRES Z ON Z.NUMSERIE = X.NUMSERIEFAC AND Z.NUMFACTURA = X.NUMFAC AND Z.N = X.N) X         
WHERE X.NUMSERIEFAC =@DocumentSeries AND X.NUMFAC = @DocumentNumber AND X.TIPODOC = @DocumentTypeId) X            
LEFT JOIN SERIESRESOLUCION W ON X.NUMRESOL = W.NUMRESOL            
LEFT JOIN TIPOSCLIENTE Z ON X.TIPOCLIENTE = (CASE 
							                     WHEN CODIGO = 1 THEN 31 
							                     WHEN CODIGO = 2 THEN 32 
							                     WHEN CODIGO = 14 THEN 44 
							                     WHEN CODIGO = 15 THEN 45  
							                     ELSE 100 --INVÁLIDO
						                      END)) F
LEFT JOIN FACTURASVENTASERIESRESOL G ON F.COMPROBANTE_SIGUIENTE = CONCAT(CONCAT('E', RIGHT(G.SERIEFISCAL2, 2)),RIGHT(CONCAT(REPLICATE('0', 10), G.NUMEROFISCAL), 10))";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            //MessageBox.Show($"Serie Documento: {DocumentSeries}, Número Documento: {DocumentNumber}, Tipo Documento: {DocumentTypeId}");

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public TaxInfo GetTaxReceiptInfo()
        {
            TaxInfo taxInformation = null;
#if !DEBUG
            try
            {
                using (var command = GetTaxReceiptInfoCmd())
                {
                    command.Connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var activoColumnIndex = reader.GetOrdinal("ACTIVO");
                        var noVencidoColumnIndex = reader.GetOrdinal("NO_VENCIDO");
                        var comprobantesRestantesColumnIndex = reader.GetOrdinal("COMPROBANTES_RESTANTES");
                        var comprobanteSiguienteColumnIndex = reader.GetOrdinal("COMPROBANTE_SIGUIENTE");
                        var tipoComprobanteColumnIndex = reader.GetOrdinal("TIPO_COMPROBANTE");
                        var comprobanteUnicoColumnIndex = reader.GetOrdinal("COMPROBANTE_UNICO");
                        var tipoClienteColumnIndex = reader.GetOrdinal("TIPO_CLIENTE");
                        var totalNetoColumnIndex = reader.GetOrdinal("TOTAL_NETO");
                        var numeroComprobanteColumnIndex = reader.GetOrdinal("NUMERO_COMPROBANTE");

                        while (reader.Read())
                        {
                            var active = reader.GetValue(activoColumnIndex).ToString() == "Y";
                            var notExpired = reader.GetValue(noVencidoColumnIndex).ToString() == "Y";
                            int.TryParse(reader.GetValue(comprobantesRestantesColumnIndex).ToString(),
                                out var remainingReceipts);
                            var nextReceipt = reader.GetValue(comprobanteSiguienteColumnIndex).ToString();
                            var receiptType = reader.GetValue(tipoComprobanteColumnIndex).ToString();
                            var uniqueReceipt = reader.GetValue(comprobanteUnicoColumnIndex).ToString() == "Y";
                            var customerType = reader.GetValue(tipoClienteColumnIndex).ToString();
                            decimal.TryParse(reader.GetValue(totalNetoColumnIndex).ToString(), out var netTotal);
                            var receiptNumber = reader.GetValue(numeroComprobanteColumnIndex).ToString();

                            taxInformation = new TaxInfo(active, notExpired, remainingReceipts, nextReceipt,
                                receiptType, uniqueReceipt, customerType, netTotal, DocumentFutureSeries,
                                receiptNumber);
                        }
                    }

                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            TaxInformation = new TaxInfo(true, true, 10, "00000000B02200055000", "CONSUMIDOR FINAL", true, "1", 0, "AC1", "");
#endif
            return taxInformation;
        }

        public List<PriceErrorDetail> GetDetailedPriceErrors()
        {
            var result = new List<PriceErrorDetail>();
#if !DEBUG
            try
            {
                using (var command = GetInvoiceErroredPricesDetailedCmd())
                {
                    command.Connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var itemCodeColumnIndex = reader.GetOrdinal("REFERENCIA");
                        var descriptionColumnIndex = reader.GetOrdinal("DESCRIPCION");
                        var priceColumnIndex = reader.GetOrdinal("PRECIO");
                        var suggestedPriceColumnIndex = reader.GetOrdinal("PRECIOSUG");

                        while (reader.Read())
                        {
                            var itemCode = reader.GetValue(itemCodeColumnIndex).ToString();
                            var description = reader.GetValue(descriptionColumnIndex).ToString();
                            var price = Convert.ToDouble(reader.GetValue(priceColumnIndex).ToString()).ToString("0.00");
                            var suggestedPrice = Convert.ToDouble(reader.GetValue(suggestedPriceColumnIndex).ToString())
                                .ToString("0.00");

                            var errorLine = new PriceErrorDetail(itemCode, description, price, suggestedPrice);
                            result.Add(errorLine);
                        }
                    }

                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            PriceErrorDetail ErrorLine = new PriceErrorDetail("460000", "BALUN", "1", "50");
            Result.Add(ErrorLine);
#endif
            return result;
        }

        private SqlCommand CheckIfRefundForInvoiceIsGreaterThan30DaysCmd()
        {
            const string query = @"
            SELECT
	            CASE WHEN DATEDIFF(DAY, C.FECHA, A.FECHA) > 30 THEN 'Y' ELSE 'N' END MAYOR_30_DIAS,
	            DATEDIFF(DAY, C.FECHA, A.FECHA) CANTIDAD_DIAS,
	            CONCAT(CONVERT(DATE, C.FECHA), '') FECHA,
	            REPLACE(B.SUPEDIDO, '.', '') NO_DOCUMENTO
            FROM ALBVENTACAB A 
            JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN AND A.N = B.N
            LEFT JOIN ALBVENTACAB C ON CONCAT('.', C.NUMSERIE, '-', C.NUMALBARAN) = B.SUPEDIDO
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId AND B.CODARTICULO <> -1
            GROUP BY C.FECHA, A.FECHA, B.SUPEDIDO";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public RefundDetail InvoiceIsGreaterThan30Days()
        {
            try
            {
                using (var command = CheckIfRefundForInvoiceIsGreaterThan30DaysCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<RefundDetail>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexDocumentId = reader.GetOrdinal("NO_DOCUMENTO");
                        var columnIndexDate = reader.GetOrdinal("FECHA");
                        var columnIndexNumberOfDays = reader.GetOrdinal("CANTIDAD_DIAS");
                        var columnIndexGreaterThan30Days = reader.GetOrdinal("MAYOR_30_DIAS");

                        while (reader.Read())
                        {
                            var documentId = reader.GetValue(columnIndexDocumentId).ToString();
                            var date = reader.GetValue(columnIndexDate).ToString();
                            var numberOfDays = Convert.ToInt32(reader.GetValue(columnIndexNumberOfDays).ToString());
                            var greaterThan30Days = reader.GetValue(columnIndexGreaterThan30Days).ToString() == "Y";

                            var refundDetail = new RefundDetail(documentId, date, numberOfDays);

                            queryRows.Add(refundDetail);
                        }
                    }

                    command.Connection.Close();

                    return queryRows[0];
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private SqlCommand CheckIfCustomClientInvoiceCmd()
        {
            const string query = @"
            SELECT
	            CASE WHEN C.PEDIR_DATOS = 'T' AND ISNULL(B.TIPO, 2) = 2 AND D.GENERAR_NCF_TRIGGER = 0 AND D.PEDIR_DATOS_CLIENTE = 1 THEN 'Y' ELSE 'N' END PEDIR_DATOS
            FROM ALBVENTACAB A 
            JOIN CLIENTES B ON A.CODCLIENTE = B.CODCLIENTE
            LEFT JOIN CLIENTESCAMPOSLIBRES C ON A.CODCLIENTE = C.CODCLIENTE
            LEFT JOIN ICGFRONT_PLUGIN_CONFIG D ON A.CAJA = D.CAJA COLLATE Latin1_General_CS_AI
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber and A.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public bool CheckIfCustomClientInvoice()
        {
            var result = false;

#if !DEBUG
            try
            {
                using (var command = CheckIfCustomClientInvoiceCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<RefundDetail>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexShouldAskForClientData = reader.GetOrdinal("PEDIR_DATOS");

                        while (reader.Read())
                            result = reader.GetValue(columnIndexShouldAskForClientData).ToString() == "Y";
                    }

                    command.Connection.Close();

                    return result;
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = true;
#endif
            return result;
        }

        private SqlCommand RemoveItbisFromRefundCmd()
        {
            const string query = @"
            UPDATE B SET 
	            B.TIPOIMPUESTO = (CASE WHEN DATEDIFF(DAY, C.FECHA, A.FECHA) > 30 THEN 3 ELSE B.TIPOIMPUESTO END),
	            B.IVA = (CASE WHEN DATEDIFF(DAY, C.FECHA, A.FECHA) > 30 THEN 0 ELSE B.IVA END)
            FROM ALBVENTACAB A 
            JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN AND A.N = B.N
            LEFT JOIN ALBVENTACAB C ON CONCAT('.', C.NUMSERIE, '-', C.NUMALBARAN) = B.SUPEDIDO
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber and A.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        private SqlCommand HasDifferentTaxCmd()
        {
            const string query = @"
            SELECT TOP 1
                CASE WHEN A.FACTORMONEDA <> C.FACTORMONEDA  THEN 'Y' ELSE 'N' END TASA_DIFERENTE,
	            A.FACTORMONEDA TASA_NC, 
	            C.FACTORMONEDA TASA_FAC,
	            REPLACE(B.SUPEDIDO, '.', '') NO_DOCUMENTO
            FROM ALBVENTACAB A 
            JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN AND A.N = B.N
            LEFT JOIN ALBVENTACAB C ON CONCAT('.', C.NUMSERIE, '-', C.NUMALBARAN) = B.SUPEDIDO
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId AND B.CODARTICULO <> -1 AND A.CODMONEDA = 2 AND C.CODMONEDA = 2 AND A.FACTORMONEDA <> C.FACTORMONEDA
            GROUP BY C.FACTORMONEDA, A.FACTORMONEDA, B.SUPEDIDO";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        } 

        public CreditNoteDetail HasDifferentTax()
        {
            try
            {
                using (var command = HasDifferentTaxCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<CreditNoteDetail>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexDocumentId = reader.GetOrdinal("NO_DOCUMENTO");
                        var columnIndexTasaNotaCredito = reader.GetOrdinal("TASA_NC");
                        var columnIndexTasaFactura = reader.GetOrdinal("TASA_FAC");
                        var columnIndexTasaDiferente = reader.GetOrdinal("TASA_DIFERENTE");

                        while (reader.Read())
                        {
                            var documentId = reader.GetValue(columnIndexDocumentId).ToString();
                            var tasaNotaCredito = reader.GetValue(columnIndexTasaNotaCredito).ToString();
                            var tasaFactura = reader.GetValue(columnIndexTasaFactura).ToString();
                            var tasaDiferente = reader.GetValue(columnIndexTasaDiferente).ToString() == "Y";

                            var refundDetail = new CreditNoteDetail(documentId, tasaNotaCredito, tasaFactura, tasaDiferente);

                            queryRows.Add(refundDetail);
                        }
                    }

                    command.Connection.Close();

                    return queryRows[0];
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private SqlCommand UpdateTaxFromCreditNoteCmd()
        {
            const string query = @"
                UPDATE A
	                SET A.FACTORMONEDA = C.FACTORMONEDA
                FROM ALBVENTACAB A 
                JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND A.NUMALBARAN = B.NUMALBARAN AND A.N = B.N
                LEFT JOIN ALBVENTACAB C ON CONCAT('.', C.NUMSERIE, '-', C.NUMALBARAN) = B.SUPEDIDO
                WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId AND B.CODARTICULO <> -1 AND A.CODMONEDA = 2 AND C.CODMONEDA = 2;";


            //MessageBox.Show("Hola DESDE EL UPDATE");
            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }



        private SqlCommand RestoreNcfCounterCmd()
        {
            const string query = @"
            UPDATE Z SET	           
	            Z.CONTADOR = Z.CONTADOR - 1
            FROM ALBVENTACAB X            
            LEFT JOIN FACTURASVENTASERIESRESOL Y ON X.NUMSERIEFAC = Y.NUMSERIE AND X.NUMFAC = Y.NUMFACTURA AND X.N = Y.N
            LEFT JOIN SERIESRESOLUCION Z ON Y.SERIEFISCAL1 = Z.SERIERESOL AND Y.SERIEFISCAL2 = Z.NUMRESOL AND Z.ACTIVO = 0
            WHERE X.NUMSERIEFAC = @DocumentSeries AND X.NUMFAC = @DocumentNumber and X.TIPODOC = @DocumentTypeId";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public void RemoveItbisFromRefund()
        {
            try
            {
                using (var command = RemoveItbisFromRefundCmd())
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //nuevo: update tax from NC
        public void UpdateTaxFromCreditNote()
        {
            try
            {
                using (var command = UpdateTaxFromCreditNoteCmd())
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void RestoreNcfCounter()
        {
            try
            {
                using (var command = RestoreNcfCounterCmd())
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private SqlCommand UpdateContributorInformationCmd(ContributorUpdatePayload updatePayload)
        {
            const string query = @"
            UPDATE B SET 
	            B.CLIF_RNC_CLIENTE = @_rnc,
	            B.CLIF_NOMBRECLIENTE = @Name,
	            B.CLIF_TIPOCLIENTE = @PaymentScheme
            FROM ALBVENTACAB A 
            JOIN FACTURASVENTACAMPOSLIBRES B ON A.NUMSERIEFAC = B.NUMSERIE AND A.NUMFAC = B.NUMFACTURA AND A.N = B.N
            JOIN CLIENTESCAMPOSLIBRES C ON A.CODCLIENTE = C.CODCLIENTE
            JOIN CLIENTES D ON A.CODCLIENTE = D.CODCLIENTE
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber and A.TIPODOC = @DocumentTypeId AND ISNULL(D.TIPO, 2) <> 1 AND C.PEDIR_DATOS = 'T'";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            command.Parameters.AddWithValue("@_rnc", SqlDbType.NVarChar).Value = updatePayload.Rnc;
            command.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = updatePayload.Name;
            command.Parameters.AddWithValue("@PaymentScheme", SqlDbType.NVarChar).Value = updatePayload.PaymentScheme;

            return command;
        }

        public bool UpdateContributorInformation(ContributorUpdatePayload updatePayload)
        {
            var result = false;
#if !DEBUG
            try
            {
                using (var command = UpdateContributorInformationCmd(updatePayload))
                {
                    command.Connection.Open();
                    result = command.ExecuteNonQuery() > 0;
                    command.Connection.Close();
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = false;
#endif
            return result;
        }

        private SqlCommand GetContributorInformationFromInvoiceCmd()
        {
            const string query = @"SELECT TOP 1
	            D.CLIF_NOMBRECLIENTE name, 
	            D.CLIF_RNC_CLIENTE rnc, 
	            D.CLIF_TIPOCLIENTE paymentScheme
            FROM ALBVENTACAB A 
            JOIN ALBVENTALIN B ON A.NUMSERIE = B.NUMSERIE AND B.NUMALBARAN = A.NUMALBARAN AND A.N = B.N
            JOIN ALBVENTACAB C ON C.NUMALBARAN = B.ABONODE_NUMALBARAN AND C.NUMSERIE = B.ABONODE_NUMSERIE AND B.N = C.N
            JOIN FACTURASVENTACAMPOSLIBRES D ON C.NUMFAC = D.NUMFACTURA AND C.NUMSERIEFAC = D.NUMSERIE AND C.N = D.N
            WHERE A.NUMSERIEFAC = @DocumentSeries AND A.NUMFAC = @DocumentNumber AND A.TIPODOC = @DocumentTypeId;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public ContributorInfo GetContributorInformationFromInvoice()
        {
#if !DEBUG
            try
            {
                using (var command = GetContributorInformationFromInvoiceCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<ContributorInfo>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexRnc = reader.GetOrdinal("rnc");
                        var columnIndexName = reader.GetOrdinal("name");
                        var columnIndexPaymentScheme = reader.GetOrdinal("paymentScheme");

                        while (reader.Read())
                        {
                            var rnc = reader.GetValue(columnIndexRnc).ToString();
                            var name = reader.GetValue(columnIndexName).ToString();
                            var paymentScheme = reader.GetValue(columnIndexPaymentScheme).ToString();
                            const string status = "2";

                            var contributorInfo = new ContributorInfo
                            {
                                Rnc = rnc,
                                Name = name,
                                PaymentScheme = paymentScheme,
                                Status = status
                            };

                            queryRows.Add(contributorInfo);
                        }
                    }

                    command.Connection.Close();

                    return queryRows[0];
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
#else
            return new ContributorInfo
            {
                rnc = "03100000001",
                name = "TEST",
                paymentScheme = "1",
                status = "2"
            };
#endif
        }

        private SqlCommand CheckIfValidPaymentFormCmd()
        {
            const string query = @"
            SELECT
	            CASE WHEN C.FORMA_PAGO_PERMITIDA IS NULL THEN 'Y' WHEN CHARINDEX(A.CODFORMAPAGO, C.FORMA_PAGO_PERMITIDA COLLATE Latin1_General_CS_AI) > 0 THEN 'Y' ELSE 'N' END FORMA_PAGO_VALIDA
            FROM TESORERIA A
            JOIN ALBVENTACAB B ON A.SERIE = B.NUMSERIEFAC AND A.NUMERO = B.NUMFAC
            LEFT JOIN CLIENTESCAMPOSLIBRES C ON A.CODIGOINTERNO = C.CODCLIENTE
            WHERE B.NUMSERIEFAC = @DocumentSeries AND B.NUMFAC = @DocumentNumber and B.TIPODOC = @DocumentTypeId AND A.ESTADO = 'S'";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public bool CheckIfValidPaymentForm()
        {
            var result = false;

#if !DEBUG
            try
            {
                using (var command = CheckIfValidPaymentFormCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<RefundDetail>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexValidPaymentForm = reader.GetOrdinal("FORMA_PAGO_VALIDA");

                        while (reader.Read()) result = reader.GetValue(columnIndexValidPaymentForm).ToString() == "Y";
                    }

                    command.Connection.Close();

                    return result;
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = true;
#endif
            return result;
        }

        private SqlCommand CheckIfPendingTreasuryLineCmd()
        {
            const string query = @"
            SELECT
	            CASE WHEN COUNT(A.CODIGOINTERNO) > 0 THEN 'Y' ELSE 'N' END LINEAS_PENDIENTE
            FROM TESORERIA A
            JOIN ALBVENTACAB B ON A.SERIE = B.NUMSERIEFAC AND A.NUMERO = B.NUMFAC
            LEFT JOIN CLIENTESCAMPOSLIBRES C ON A.CODIGOINTERNO = C.CODCLIENTE
            WHERE B.NUMSERIEFAC = @DocumentSeries AND B.NUMFAC = @DocumentNumber and B.TIPODOC = @DocumentTypeId AND A.ESTADO = 'P'";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentNumber", SqlDbType.Int).Value = DocumentNumber;
            command.Parameters.AddWithValue("@DocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;

            return command;
        }

        public bool CheckIfPendingTreasuryLine()
        {
            var result = false;

#if !DEBUG
            try
            {
                using (var command = CheckIfPendingTreasuryLineCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<RefundDetail>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexPendingLines = reader.GetOrdinal("LINEAS_PENDIENTE");

                        while (reader.Read()) result = reader.GetValue(columnIndexPendingLines).ToString() == "Y";
                    }

                    command.Connection.Close();

                    return result;
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = true;
#endif
            return result;
        }

        private SqlCommand GetContributorInfoCmd(string rnc)
        {
            const string query = "SELECT rnc, name, paymentScheme, status FROM DGII_CONTRIBUYENTES WHERE rnc = @_rnc";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@_rnc", SqlDbType.NVarChar).Value = rnc;

            return command;
        }

        public ContributorInfo GetContributorInfo(string rnc)
        {
            try
            {
                using (var command = GetContributorInfoCmd(rnc))
                {
                    command.Connection.Open();

                    var queryRows = new List<ContributorInfo>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexRnc = reader.GetOrdinal("rnc");
                        var columnIndexName = reader.GetOrdinal("name");
                        var columnIndexStatus = reader.GetOrdinal("status");

                        while (reader.Read())
                        {
                            var contributorInfo = new ContributorInfo
                            {
                                Rnc = reader.GetValue(columnIndexRnc).ToString(),
                                Name = reader.GetValue(columnIndexName).ToString(),
                                PaymentScheme = "2",
                                Status = reader.GetValue(columnIndexStatus).ToString()
                            };

                            queryRows.Add(contributorInfo);
                        }
                    }

                    command.Connection.Close();

                    return queryRows[0];
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        private SqlCommand GetCurrentRateCmd()
        {
            const string query = "SELECT D.COTIZACION FROM COTIZACIONES D WHERE D.CODMONEDA = 2 ORDER BY D.FECHA DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;";

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            return command;
        }

        public decimal GetCurrentRate()
        {
            decimal result = 0;

#if !DEBUG
            try
            {
                using (var command = GetCurrentRateCmd())
                {
                    command.Connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexRate = reader.GetOrdinal("COTIZACION");

                        while (reader.Read()) result = Convert.ToDecimal(reader.GetValue(columnIndexRate).ToString());
                    }

                    command.Connection.Close();

                    return result;
                }
            }
            catch (Exception)
            {
                // ignored
            }
#else
            Result = 57.5M;
#endif
            return result;
        }

        private SqlCommand GetInvalidDocumentsCmd()
        {
            const string query = @"
            SELECT 
	            B.SERIEFISCAL2 TaxSeries, 
	            B.NUMEROFISCAL TaxNum, 
	            A.NUMSERIE Series,
	            A.NUMFAC Number,
	            C.ALIAS CardCode,
	            C.NOMBRECLIENTE CardName,                                 
	            CONVERT(VARCHAR, A.FECHA, 23) DocDate,
	            D.INICIALES DocCur,
	            FORMAT(A.TOTALNETO, '#,0.00') DocTotal
            FROM ALBVENTACAB A
            JOIN FACTURASVENTASERIESRESOL B ON A.NUMFAC = B.NUMFACTURA AND A.NUMSERIEFAC = B.NUMSERIE 
            LEFT JOIN CLIENTES C ON A.CODCLIENTE = C.CODCLIENTE
            LEFT JOIN MONEDAS D ON A.CODMONEDA = D.CODMONEDA
            WHERE A.Z = (SELECT TOP 1 NUMERO + 1 FROM ARQUEOS WHERE CERRADO = 1 AND ARQUEO = 'Z' 
            AND CAJA = A.CAJA ORDER BY NUMERO DESC) AND (A.NUMSERIE = @DocumentSeries OR A.NUMSERIE = @DocumentFutureSeries)"; //AND LEN(B.NUMEROFISCAL) < 9

            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = query
            };

            command.Parameters.AddWithValue("@DocumentSeries", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@DocumentFutureSeries", SqlDbType.NVarChar).Value = DocumentFutureSeries;

            return command;
        }

        public List<ErroredDocumentDetail> GetInvalidDocuments()
        {
            try
            {
                using (var command = GetInvalidDocumentsCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<ErroredDocumentDetail>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexSeries = reader.GetOrdinal("Series");
                        var columnIndexNumber = reader.GetOrdinal("Number");
                        var columnIndexCardCode = reader.GetOrdinal("CardCode");
                        var columnIndexCardName = reader.GetOrdinal("CardName");
                        var columnIndexDocDate = reader.GetOrdinal("DocDate");
                        var columnIndexDocCur = reader.GetOrdinal("DocCur");
                        var columnIndexDocTotal = reader.GetOrdinal("DocTotal");

                        while (reader.Read())
                        {
                            var series = reader.GetValue(columnIndexSeries).ToString();
                            var number = Convert.ToInt32(reader.GetValue(columnIndexNumber).ToString());
                            var cardCode = reader.GetValue(columnIndexCardCode).ToString();
                            var cardName = reader.GetValue(columnIndexCardName).ToString();
                            var docDate = reader.GetValue(columnIndexDocDate).ToString();
                            var docCur = reader.GetValue(columnIndexDocCur).ToString();
                            var docTotal = Convert.ToDouble(reader.GetValue(columnIndexDocTotal).ToString());

                            var errorDocument = new ErroredDocumentDetail(
                                series,
                                number,
                                cardCode,
                                cardName,
                                docDate,
                                docCur,
                                docTotal
                            );

                            queryRows.Add(errorDocument);
                        }
                    }

                    command.Connection.Close();

                    return queryRows;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        private SqlCommand GenerateRaffleTicketsCmd()
        {
            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = "GENERATE_RAFFLE_NUMBERS",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@NUMSERIE", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@NUMFAC", SqlDbType.Int).Value = DocumentNumber;

            return command;
        }

        public List<RaffleTicket> GenerateRaffleTickets()
        {
            if (!DocumentSeries.Contains("F"))
                return null;

#if !DEBUG
            try
            {
                using (var command = GenerateRaffleTicketsCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<RaffleTicket>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexCode = reader.GetOrdinal("Code");
                        var columnIndexSeries = reader.GetOrdinal("InvoiceSeries");
                        var columnIndexNumber = reader.GetOrdinal("InvoiceNumber");
                        var columnIndexPromotionId = reader.GetOrdinal("PromotionId");
                        var columnIndexPromotionName = reader.GetOrdinal("PromotionName");
                        var columnIndexPromotionInfo = reader.GetOrdinal("PromotionInfo");
                        var columnIndexPromotionSd = reader.GetOrdinal("PromotionStartDate");
                        var columnIndexPromotionEd = reader.GetOrdinal("PromotionEndDate");

                        while (reader.Read())
                        {
                            var code = reader.GetValue(columnIndexCode).ToString();
                            var series = reader.GetValue(columnIndexSeries).ToString();
                            var number = Convert.ToInt32(reader.GetValue(columnIndexNumber).ToString());
                            var promotionId = Convert.ToInt32(reader.GetValue(columnIndexPromotionId).ToString());
                            var promotionName = reader.GetValue(columnIndexPromotionName).ToString();
                            var promotionInfo = reader.GetValue(columnIndexPromotionInfo).ToString();
                            var promotionStartDate = DateTime.Parse(reader.GetValue(columnIndexPromotionSd).ToString());
                            var promotionEndDate = DateTime.Parse(reader.GetValue(columnIndexPromotionEd).ToString());

                            var raffleTicket = new RaffleTicket(
                                code,
                                series,
                                number,
                                promotionId,
                                promotionName,
                                promotionInfo,
                                promotionStartDate,
                                promotionEndDate
                            );

                            queryRows.Add(raffleTicket);
                        }
                    }

                    command.Connection.Close();

                    return queryRows;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
#else
            return new List<RaffleTicket>
            {
                new RaffleTicket(
                    "XXX1-YYYY-ZZZZ",
                    "AB1F",
                    0,
                    0,
                    "GRAN RIFA DAHUA!",
                    "Gracias por preferirnos!",
                    DateTime.Parse("2022-10-05"),
                    DateTime.Parse("2022-10-31")
                ),
                new RaffleTicket(
                    "XXX2-YYYY-ZZZZ",
                    "AB1F",
                    0,
                    0,
                    "GRAN RIFA DAHUA!",
                    "Gracias por preferirnos!",
                    DateTime.Parse("2022-10-05"),
                    DateTime.Parse("2022-10-31")
                )
            };
#endif
        }

        private SqlCommand GetRaffleTicketsCmd()
        {
            var command = new SqlCommand
            {
                Connection = GetSqlConnection(),
                CommandText = "GET_RAFFLE_NUMBERS",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@NUMSERIE", SqlDbType.NVarChar).Value = DocumentSeries;
            command.Parameters.AddWithValue("@NUMFAC", SqlDbType.Int).Value = DocumentNumber;

            return command;
        }

        public List<RaffleTicket> GetRaffleTickets()
        {
#if !DEBUG
            try
            {
                using (var command = GetRaffleTicketsCmd())
                {
                    command.Connection.Open();

                    var queryRows = new List<RaffleTicket>();

                    using (var reader = command.ExecuteReader())
                    {
                        var columnIndexCode = reader.GetOrdinal("Code");
                        var columnIndexSeries = reader.GetOrdinal("InvoiceSeries");
                        var columnIndexNumber = reader.GetOrdinal("InvoiceNumber");
                        var columnIndexPromotionId = reader.GetOrdinal("PromotionId");
                        var columnIndexPromotionName = reader.GetOrdinal("PromotionName");
                        var columnIndexPromotionInfo = reader.GetOrdinal("PromotionInfo");
                        var columnIndexPromotionSd = reader.GetOrdinal("PromotionStartDate");
                        var columnIndexPromotionEd = reader.GetOrdinal("PromotionEndDate");

                        while (reader.Read())
                        {
                            var code = reader.GetValue(columnIndexCode).ToString();
                            var series = reader.GetValue(columnIndexSeries).ToString();
                            var number = Convert.ToInt32(reader.GetValue(columnIndexNumber).ToString());
                            var promotionId = Convert.ToInt32(reader.GetValue(columnIndexPromotionId).ToString());
                            var promotionName = reader.GetValue(columnIndexPromotionName).ToString();
                            var promotionInfo = reader.GetValue(columnIndexPromotionInfo).ToString();
                            var promotionStartDate = DateTime.Parse(reader.GetValue(columnIndexPromotionSd).ToString());
                            var promotionEndDate = DateTime.Parse(reader.GetValue(columnIndexPromotionEd).ToString());

                            var raffleTicket = new RaffleTicket(
                                code,
                                series,
                                number,
                                promotionId,
                                promotionName,
                                promotionInfo,
                                promotionStartDate,
                                promotionEndDate
                            );

                            queryRows.Add(raffleTicket);
                        }
                    }

                    command.Connection.Close();

                    return queryRows;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
#else
            return new List<RaffleTicket>
                {
                    new RaffleTicket(
                        "XXX1-YYYY-ZZZZ",
                        "AB1F",
                        0,
                        0,
                        "GRAN RIFA DAHUA!",
                        "Gracias por preferirnos!",
                        DateTime.Parse("2022-10-05"),
                        DateTime.Parse("2022-10-31")
                    ),
                    new RaffleTicket(
                        "XXX2-YYYY-ZZZZ",
                        "AB1F",
                        0,
                        0,
                        "GRAN RIFA DAHUA!",
                        "Gracias por preferirnos!",
                        DateTime.Parse("2022-10-05"),
                        DateTime.Parse("2022-10-31")
                    )
                };
#endif
        }
    }


    public static class Utilities
    {
        private const string DefaultXmlFile = "ICGPluginParameters.xml";
        private const string DefaultIniFile = "CSICGImpFiscal.ini";

        public static ParsedDocument ParseXml()
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(DefaultXmlFile);
                 
                // Database credentials.
                var serverName = xmlDocument.SelectSingleNode("//bd//server")?.InnerText;
                var databaseName = xmlDocument.SelectSingleNode("//bd/database")?.InnerText;
                var userName = xmlDocument.SelectSingleNode("//bd/user")?.InnerText;

                // Document information.
                var documentType = xmlDocument.SelectSingleNode("//tipodoc")?.InnerText;
                var documentSeries = xmlDocument.SelectSingleNode("//serie")?.InnerText;
                var documentNumber = int.Parse(xmlDocument.SelectSingleNode("//numero")?.InnerText);
                var documentN = xmlDocument.SelectSingleNode("//n")?.InnerText;
                var documentFutureSeries = "";


                // [ENMANUEL]: Vendedor Information.
                var codVendedor = xmlDocument.SelectSingleNode("//codvendedor")?.InnerText;

                try
                {
                    var futureSeries = xmlDocument.SelectSingleNode("//futuraserie");

                    if (futureSeries != null)
                        documentFutureSeries = futureSeries.InnerText;
                }
                catch (Exception)
                {
                    Console.WriteLine(@"Could not get 'futuraserie'. Skipping...");
                }

                var documentTypeId = 5;

                switch (documentType)
                {
                    case "FACVENTA":
                    {
                        if (documentFutureSeries.EndsWith("H"))
                            documentTypeId = 17;
                        break;
                    }
                    case "ALBVENTA":
                        documentTypeId = 3;
                        break;
                    case "PEDVENTA":
                        documentTypeId = 1;
                        break;
                }

                return new ParsedDocument(serverName, databaseName, documentType, documentTypeId, documentSeries,
                    documentNumber, documentN, documentFutureSeries, codVendedor);
            }
            catch (Exception exception)
            {
                Console.WriteLine(@"Error parsing XML file: " + exception.Message);
            }

            return null;
        }

        public static bool UpdateXml(bool throwError = false, string errorMessage = null)
        {
            var success = true;

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(DefaultXmlFile);
                XmlNode parent = xmlDocument.DocumentElement;

                var externalExecution = parent.SelectSingleNode("//ejecucionexterna");

                if (externalExecution == null)
                {
                    externalExecution = xmlDocument.CreateNode(XmlNodeType.Element, "ejecucionexterna", "");
                    parent.AppendChild(externalExecution);
                }

                var correctExecution = externalExecution.SelectSingleNode("//correcta");

                if (correctExecution == null)
                {
                    correctExecution = xmlDocument.CreateNode(XmlNodeType.Element, "correcta", "");
                    externalExecution.AppendChild(correctExecution);
                }

                if (throwError)
                {
                    if (errorMessage != null)
                    {
                        var msg = xmlDocument.CreateNode(XmlNodeType.Element, "msg", "");
                        msg.InnerText = errorMessage;
                        parent.AppendChild(msg);
                    }

                    var incorrectAction = externalExecution.SelectSingleNode("//accionsiincorrecta") ?? xmlDocument.CreateNode(XmlNodeType.Element, "accionsiincorrecta", "");

                    incorrectAction.InnerText = "1";
                    externalExecution.AppendChild(incorrectAction);

                    correctExecution.InnerText = "0";
                }
                else
                {
                    correctExecution.InnerText = "1";
                }

                xmlDocument.Save(DefaultXmlFile);
            }
            catch (Exception exception)
            {
                Console.WriteLine(@"Error updating XML file: " + exception.Message);

                success = false;
            }

            return success;
        }

        public static string GetMd5Hash(string text)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(text);
            var hash = md5.ComputeHash(inputBytes);
            var stringBuilder = new StringBuilder();

            foreach (var t in hash)
                stringBuilder.Append(t.ToString("x2"));

            return stringBuilder.ToString();
        }

        public static DialogResult ShowAlert(string message, string title = "Informacion",
            MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }

        public static Dictionary<string, string> ParseIniFile()
        {
            char[] separator = { '=' };

            var rawFile = File.ReadAllLines(DefaultIniFile);

            return (from line in rawFile where !line.StartsWith(";") && !line.StartsWith(" ") && !line.StartsWith("[") && line.Length != 0 select line.Split(separator, 2) into keyValuePair where keyValuePair.Length == 2 select keyValuePair).ToDictionary(keyValuePair => keyValuePair[0], keyValuePair => keyValuePair[1]);
        }

        public static string GetFiscalPrinterPort()
        {
            try
            {
                var iniFile = ParseIniFile();

                var portNumber = iniFile.FirstOrDefault(i => i.Key == "Puerto").Value ?? "2";
                return $@"COM{portNumber}";
            }
            catch
            {
                return "COM1";
            }
        }

        public static bool PrintRaffleTicketsThroughFiscalPrinter()
        {
            try
            {
                var iniFile = ParseIniFile();

                var printThroughFiscalPrinter =
                    iniFile.FirstOrDefault(i => i.Key == "ImprimirBoletosRifa").Value ?? "0";
                return printThroughFiscalPrinter == "1";
            }
            catch
            {
                return false;
            }
        }

    }
}