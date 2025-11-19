using System;
using Plugin_ICGFront.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugin_ICGFront.Views
{
    public partial class SerialsInvoice : Form
    {
        private readonly ParsedDocument _parsedDocument;
         
        public string invalid_cell = "###########";//VALOR PARA CELDA INVÁLIDA
        public Color duplicated_color = Color.LightPink;//COLOR PARA CELDA DUPLICADA
        public Color found_color = Color.LightGreen;//COLOR PARA CELDAS ENCONTRADAS
        public Color back_ground_invalid_cell = Color.LightCoral;//COLOR PARA CELDAS INVÁLIDAS
        public Color back_ground_valid_cell = Color.LightCyan;//COLOR PARA CELDAS VÁLIDAS

        List<Articulo_Cantidad> articulos;//TRAE EL CÓDIGO DEL ARTÍCULO Y SU CANTIDAD DE ARTÍCULOS
        private int cantidad_mayor_articulos;


        public SerialsInvoice()
        {
            InitializeComponent();
            this.btn_guardar_cerrar.Enabled = false;
        }

        private void SerialsInvoice_Load(object sender, EventArgs e)
        {
            
        }

        private void PreparaDataGridView()
        {
            int cant_kits = _parsedDocument.Cantidad_Kits_Factura();//OBTENER CANTIDAD DE KITs DE FACTURA
            this.articulos = _parsedDocument.Obtener_Articulos_Cantidad();//OBTENER CANTIDAD DE AR

            cantidad_mayor_articulos = _parsedDocument.obtener_cantidad_maxima_articulos_kit();

            if (cantidad_mayor_articulos < 0)
            {
                MessageBox.Show("No se pudo obtener la cantidad máxima de artículos.");
                return;
            }

            if (articulos.Count == 0)
            {
                MessageBox.Show("No se encontraron artículos.");
                return;
            }

            // Limpiar el DataGridView para reiniciarlo
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            // Crear las columnas a partir de los artículos (SOLO COLUMNAS)
            foreach (var item in articulos)
            {
                if (!dataGridView1.Columns.Contains(item.CODARTICULO))
                {
                    dataGridView1.Columns.Add(item.CODARTICULO, item.CODARTICULO);
                }
            }

            int maxCantidad = articulos.Max(a => a.CANTIDAD);

            // Añadir filas según la cantidad máxima de artículos (SOLO FILAS)
            for (int i = 0; i <= maxCantidad; i++)
            {
                dataGridView1.Rows.Add();
            }

            dataGridView1.AllowUserToAddRows = false; // Desactivar la adición manual de filas

            // Obtener nuevamente la cantidad máxima de artículos
            int maxCantidad_mayor = _parsedDocument.obtener_cantidad_maxima_articulos_kit();

            // Sombrear campos y preparar las celdas
            foreach (var item in articulos)
            {
                for (int i = 0; i < maxCantidad_mayor + 1; i++)
                {
                    // Sombrear en coral las celdas no editables 
                    dataGridView1[item.CODARTICULO, i].Style.BackColor = back_ground_invalid_cell;
                    dataGridView1[item.CODARTICULO, i].Value = this.invalid_cell;
                    dataGridView1[item.CODARTICULO, i].Style.ForeColor = Color.DimGray;
                    dataGridView1[item.CODARTICULO, i].ReadOnly = true;
                }

                // Habilitar las celdas que deben ser editables
                for (int i = 0; i < item.CANTIDAD; i++)
                {
                    dataGridView1[item.CODARTICULO, i].Style.BackColor = back_ground_valid_cell;
                    dataGridView1[item.CODARTICULO, i].Value = "";
                    dataGridView1[item.CODARTICULO, i].ReadOnly = false;
                }
            }

            ValidarDuplicadosDataGridView();
        }

        private void ValidarDuplicadosDataGridView()
        {
            Dictionary<string, List<DataGridViewCell>> valores = new Dictionary<string, List<DataGridViewCell>>();

            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    string valorCelda = celda.Value?.ToString() ?? "";

                    // Ignorar celdas vacías
                    if (string.IsNullOrWhiteSpace(valorCelda))
                    {
                        continue;
                    }

                    // Agrupar celdas por valor
                    if (valores.ContainsKey(valorCelda))
                    {
                        valores[valorCelda].Add(celda);
                    }
                    else
                    {
                        valores[valorCelda] = new List<DataGridViewCell> { celda };
                    }
                }
            }

            // Marcar celdas duplicadas en rojo
            foreach (var item in valores)
            {
                if (item.Value.Count > 1) // Si hay más de una celda con el mismo valor
                {
                    foreach (DataGridViewCell encontrado in item.Value)
                    {
                        encontrado.Style.BackColor = Color.Red;
                    }
                }
            }
        }

        private void VerificarDuplicados()
        {
            // Crear un diccionario para almacenar las ocurrencias de cada valor
            Dictionary<string, List<DataGridViewCell>> valores = new Dictionary<string, List<DataGridViewCell>>();

            // Limpiar el formato previo
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    if (celda.Style.BackColor == found_color)
                    {
                        //nothing
                    }
                    else
                    {
                        celda.Style.BackColor = back_ground_valid_cell; // Restaurar color por defecto
                    }

                }
            }

            // Recorrer las celdas del DataGridView
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    string valorCelda = celda.Value?.ToString() ?? "";

                    // Ignorar las celdas vacías
                    if (string.IsNullOrWhiteSpace(valorCelda))
                    {
                        continue;
                    }

                    // Si ya se ha encontrado ese valor, agregar la celda a la lista de duplicados
                    if (valores.ContainsKey(valorCelda))
                    {
                        valores[valorCelda].Add(celda);
                    }
                    else
                    {
                        valores[valorCelda] = new List<DataGridViewCell> { celda };
                    }
                }
            }

            // Marcar todas las celdas que tienen valores duplicados
            foreach (var item in valores)
            {
                if (item.Value.Count > 1) // Si hay más de una celda con el mismo valor
                {
                    foreach (DataGridViewCell celdaDuplicada in item.Value)
                    {
                        celdaDuplicada.Style.BackColor = duplicated_color; // Marcar en color distintivo
                    }
                }
            }
        }

        // Evento que puedes asociar al cambiar alguna celda o verificar al cargar
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            VerificarDuplicados(); // Llamar a la función cada vez que cambie el valor de una celda
        }

        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {
            VerificarDuplicados();
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            if (Buscar() >= 1)
            {

            }
            else
            {
                VerificarDuplicados();
            }
        }

        private int Buscar()
        {
            int contador = 0;
            string buscar = this.txt_texto_buscar.Text.Trim(); // Si el texto de búsqueda está en el botón, cámbialo por el TextBox
            // Recorrer todas las celdas para encontrar las coincidencias
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    // Obtener el valor de la celda como cadena
                    string valorCelda = celda.Value?.ToString() ?? "";
                    // Ignorar las celdas vacías
                    if (string.IsNullOrWhiteSpace(valorCelda))
                    {
                        continue;
                    }

                    // Comparar el valor de la celda con el valor de búsqueda (sin importar mayúsculas o minúsculas)
                    if (valorCelda.Equals(buscar, StringComparison.OrdinalIgnoreCase))
                    {
                        // Marcar en azul claro las celdas encontradas
                        celda.Style.BackColor = found_color;
                        contador++;
                    }
                    else
                    {
                        if (celda != null
                            && celda.ReadOnly == true)
                        {
                            //nothing
                        }
                        else
                        {
                            if (celda.Style.BackColor == duplicated_color)
                            {
                                //nothing
                            }
                            else
                            {
                                celda.Style.BackColor = Color.White;
                            }

                        }

                        if (celda.Style.BackColor == found_color)
                        {
                            celda.Style.BackColor = Color.White;
                        }
                    }
                }
            }

            return contador;
        }

        private void txt_texto_buscar_TextChanged(object sender, EventArgs e)
        {
            if (Buscar() >= 1)
            {

            }
            else
            {
                VerificarDuplicados();
            }
        }
         
        private void btn_limpiar_columna_Click(object sender, EventArgs e)
        {
            int col_seleccionada = dataGridView1.CurrentCell.ColumnIndex;

            // Recorrer todas las filas
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                // Obtener la celda de la columna seleccionada en la fila actual
                DataGridViewCell celda = fila.Cells[col_seleccionada];

                // Verificar si la celda tiene un valor y no está vacía
                if (celda.Value != null && celda.Value.ToString() != "" && celda.Value.ToString() != invalid_cell)
                {
                    // Limpiar el valor de la celda
                    celda.Value = "";
                }
            }
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_limpiar_todo_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    if (celda.Value != null && celda.Value.ToString() != "" && celda.Value.ToString() != invalid_cell)
                    {
                        celda.Value = "";
                    }
                }
            }
        }

        private void btn_revisar_todo_Click(object sender, EventArgs e)
        {
            this.txt_mensajes_error.Text = string.Empty;//limpiamos los mensajes de error 

            if (BuscandoErrores() == 1)
            {
                this.btn_guardar_cerrar.Enabled = true;
            }
            else
            {


            }

        }

        public int BuscandoErrores()
        {
            // Inicializar el mensaje de error
            StringBuilder mensajeError = new StringBuilder();


            //contando cantidad de celdas válidas
            int count_valid_cells = 0, count_total = 0;
            int count_null = 0, count_vacios = 0, count_invalid = 0;

            char[] signos_especiales_permitidos = { '-', '#' };
            string[] sqlKeywords = { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "EXEC", "UNION" };


            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    count_total++;

                    string valorCelda = celda.Value != null ? celda.Value.ToString().Trim() : "";


                    if (celda.Value.ToString() != invalid_cell)
                    {
                        count_valid_cells++;//aquí solo cuento las celdas válidas
                    }

                    //contando celdas inválidas
                    if (celda.Value == null)
                    {
                        count_null++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(valorCelda))
                    {
                        count_vacios++;
                        continue;
                    }

                    if (valorCelda != invalid_cell)
                    {
                        count_valid_cells++;
                    }

                    if (valorCelda.Length <= 5)
                    {
                        count_invalid++;
                        continue;
                    }

                    if (valorCelda.Distinct().Count() == 1 && valorCelda != invalid_cell)
                    {
                        count_invalid++;
                        continue;
                    }

                    if (valorCelda.Contains("'") || valorCelda.Contains("\"") || valorCelda.Contains(";"))
                    {
                        count_invalid++;
                        continue;
                    }

                    if (valorCelda.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)) &&
                        !valorCelda.Any(c => signos_especiales_permitidos.Contains(c)))
                    {
                        count_invalid++;
                        continue;
                    }

                    if (sqlKeywords.Any(keyword => valorCelda.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        count_invalid++;
                        continue;
                    }

                }
            }


            if (count_vacios > 0)
            {
                mensajeError.AppendLine($"Celdas vacías: {count_vacios}");
            }

            if (count_null > 0)
            {
                mensajeError.AppendLine($"Celdas nulas: {count_null}");
            }

            if (count_invalid > 0)
            {
                mensajeError.AppendLine($"Celdas inválidas (más de 5 caracteres, caracteres iguales, inyección SQL, signos especiales): {count_invalid}");
            }

            // Detallar las celdas con errores
            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                foreach (DataGridViewCell celda in fila.Cells)
                {
                    string valorCelda = celda.Value?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(valorCelda))
                    {
                        continue;
                    }

                    if ((valorCelda.Length <= 5
                        || valorCelda.Distinct().Count() == 1
                        || sqlKeywords.Any(keyword => valorCelda.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        || valorCelda.Any(ch => !char.IsLetterOrDigit(ch) && ch != '-' && ch != '#'))
                        && valorCelda.ToString() != invalid_cell
                        )
                    {
                        // Agregar la celda con error
                        mensajeError.AppendLine($"Error en fila {celda.RowIndex + 1}, columna {celda.ColumnIndex + 1}: {valorCelda}");
                    }

                }
            }
            if (count_null > 0 || count_vacios > 0 || count_invalid > 0)
            {
                // Asignar el mensaje de error al TextBox
                this.txt_mensajes_error.Text = mensajeError.ToString();
                return 0;
            }
            else
            {
                return 1;
            }



        }


        private void btn_guardar_cerrar_Click(object sender, EventArgs e)
        {
            StringBuilder datos = new StringBuilder();

            for (int i = 0; i < dataGridView1.Columns.Count; i++)//Convertimos los datos de las columnas a una sola cadena con la estructura espedificada
            {
                datos.Append(dataGridView1.Columns[i].HeaderText.Trim() + ":");

                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    //string valorCelda = celda.Value?.ToString() ?? "";
                    //valorCelda.ToString() != invalid_cell
                    if (!dataGridView1.Rows[j].IsNewRow && (dataGridView1.Rows[j].Cells[i].Value?.ToString().Trim() ?? string.Empty) != invalid_cell)
                    {
                        datos.Append(dataGridView1.Rows[j].Cells[i].Value?.ToString().Trim() ?? string.Empty);

                        if (j < dataGridView1.Rows.Count - 2)
                        {
                            datos.Append(",");
                        }
                    }
                }
                if (!(i == dataGridView1.Columns.Count - 1))
                {
                    datos.Append(";");
                }
            }

            string datosFinales = datos.ToString();

            DialogResult resultado = MessageBox.Show("La cadena resultante es: " + datosFinales + " ¿Desea Guardarla?", "Confirmación", MessageBoxButtons.YesNo);

            if (resultado == DialogResult.Yes)
            {
                Insertar(datosFinales);//insertar datos
            }


            //este método estará descontinuado cuando adapte la prueba 4
            /*StringBuilder datos = new StringBuilder();

            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                if (!fila.IsNewRow) // Para evitar la fila vacía que aparece al final
                {
                    for (int i = 0; i < fila.Cells.Count; i++)
                    {

                        datos.Append(fila.Cells[i].Value?.ToString() ?? string.Empty);
                        if (i < fila.Cells.Count - 1)
                        {
                            datos.Append(","); // Separador de columnas
                        }
                    }
                    datos.AppendLine(";"); // Separador de filas
                }
            }

            string datosFinales = datos.ToString();*/

        }

        private void Insertar(String datos)
        {

            List<String> sub_cadenas = SubCadenas(datos);//obtenemos los datos de las columnas en una sola cadena

            if (_parsedDocument.InsertarSeriales(sub_cadenas))
            {
                MessageBox.Show("Insertado Correctamente!");
            }
            else
            {
                MessageBox.Show("Error al insertar.");
            }
        }

        //método para dividir las cadenas que superen los 4000 caracteres en varias subcadenas
        private List<String> SubCadenas(string cadena)
        {
            string cadenaOriginal = cadena;
            int maxCaracteres = 4000;
            List<string> subCadenas = new List<string>();

            string[] columnas = cadenaOriginal.Split(';');

            string subCadenaActual = "";
            int longitudActual = 0;

            foreach (string columna in columnas)
            {
                string columnaConSeparador = columna + ";";

                // Si agregar esta columna excede el límite de caracteres
                if (longitudActual + columnaConSeparador.Length > maxCaracteres)
                {
                    // Guardar la subcadena actual en la lista y reiniciar para la siguiente
                    subCadenas.Add(subCadenaActual);
                    subCadenaActual = ""; // Reiniciar la subcadena
                    longitudActual = 0;
                }

                // Agregar la columna actual a la subcadena
                subCadenaActual += columnaConSeparador;
                longitudActual += columnaConSeparador.Length;
            }

            // Agregar la última subcadena si no está vacía
            if (!string.IsNullOrEmpty(subCadenaActual))
            {
                subCadenas.Add(subCadenaActual);
            }

            return subCadenas;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string datosRecuperados = RecuperarDatos();
            CargarDatosEnDataGridView(datosRecuperados);
        }

        private string RecuperarDatos()
        {
            string datos = string.Empty;

            datos = _parsedDocument.RecuperarDatos();

            return datos;
        }

        private void CargarDatosEnDataGridView(string datos1)
        {
            string cadena = datos1;

            //dataGridView1.DataSource = new List<object>();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();


            string[] columnas = cadena.Split(';');

            List<string[]> datosPorColumna = new List<string[]>();
            int maxFilas = 0;

            foreach (string columna in columnas)
            {
                string[] partes = columna.Split(':');
                string nombreColumna = partes[0];
                string[] datos = partes[1].Split(',');

                dataGridView1.Columns.Add(nombreColumna, nombreColumna);

                datosPorColumna.Add(datos);
                maxFilas = Math.Max(maxFilas, datos.Length);
            }

            for (int i = 0; i < maxFilas; i++)
            {
                var fila = new List<string>();

                foreach (var datos in datosPorColumna)
                {
                    fila.Add(i < datos.Length ? datos[i] : invalid_cell);
                }

                dataGridView1.Rows.Add(fila.ToArray());
            }

            Desactivar_Celdas();
        }

        private void Desactivar_Celdas()
        {

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value.ToString() == invalid_cell)
                    {
                        cell.Style.BackColor = back_ground_invalid_cell;
                        cell.Style.ForeColor = Color.DimGray;
                        cell.ReadOnly = true;
                    }
                }

            }
        }

    }
}
