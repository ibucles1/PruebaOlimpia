using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Multiplo
{
    public partial class ValidateMultplo : Form
    {

        private string rutaArchivo;
        private enum TipoMensaje { Exitoso = 1, Validacion = 2, Error = 3 };
        public ValidateMultplo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            openFileDialog1.Title = "Seleccione un archivo Csv o Txt para realizar validacion.";
            openFileDialog1.Filter = "Archivos txt (*.txt)|*.txt|Archivos CSV (*.csv)|*.csv";
            openFileDialog1.FileName = string.Empty;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PathResultFile.Visible = false;
                    PathResultFile.Text = string.Empty;

                    rutaArchivo = openFileDialog1.FileName;
                    string pathDirectory = Path.GetDirectoryName(rutaArchivo);
                    openFileDialog1.InitialDirectory = pathDirectory;
                    textBox1.Text = rutaArchivo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    ShowMessage(TipoMensaje.Error, "");
                }
                finally
                {
                    Console.WriteLine("Finalizacion de codigo.");
                }
            }

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (File.Exists(rutaArchivo) || File.Exists(textBox1.Text))
            {
                string pathDirectory = Path.GetDirectoryName(rutaArchivo);
                openFileDialog1.InitialDirectory = pathDirectory;
                DateTime date = DateTime.Now;
                var nameNewFile = "result" + " - " + date.ToString("yyyyMMddHHmmss") + ".txt";
                var finalPathNewFile = Path.Combine(pathDirectory, nameNewFile);

                var stream = openFileDialog1.OpenFile();

                ValidateFile(finalPathNewFile, stream);
            }
            else
                ShowMessage(TipoMensaje.Validacion, "Debe seleccionar o ingresar una ruta de archivo valida.");

        }

        private void ValidateFile(string path, Stream fileStream)
        {

            string fileContent;

            //Read the contents of the file into a stream   

            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContent = reader.ReadToEnd();
            }

            var array = fileContent.Split("\r\n");

            if (array.Length > 0)
            {
                foreach (var line in array)
                {
                    var result = Validate(line.Trim());
                    WriteFile(result, path);
                }

                ShowMessage(TipoMensaje.Exitoso, "");
                PathResultFile.Visible = true;
                PathResultFile.Text = path;
            }

        }

        private bool Validate(string num)
        {
            Regex regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(num))
                return false;
            else
            {
                if (num.Length >= 50 && num.Length <= 1000)
                {
                    var suma = 0;

                    for (int j = 0; j < num.Length; j++)
                    {
                        var numero = num.Substring(j, 1);
                        suma += int.Parse(numero);
                    }

                    if (suma % 3 == 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false; ;
            }
        }

        private void WriteFile(bool result, string path)
        {

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    if (result)
                        sw.WriteLine("SI");

                    else
                        sw.WriteLine("NO");

                }
            }
            else
            {
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (result)
                            sw.WriteLine("SI");
                        else
                            sw.WriteLine("NO");

                    }
                }
            }

        }

        private void ShowMessage(TipoMensaje tipoMensaje, string message)
        {

            switch (tipoMensaje)
            {
                case TipoMensaje.Exitoso:
                    MessageBox.Show("Proceso finalizado.", "Exitoso");
                    break;
                case TipoMensaje.Validacion:
                    MessageBox.Show(message, "Validacion");
                    break;
                case TipoMensaje.Error:
                    MessageBox.Show("Ha ocurrido un error.", "Error");
                    break;
                default:
                    break;
            }
        }

        private void Limpiar(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            openFileDialog1.FileName = string.Empty;
            PathResultFile.Visible = false;
            PathResultFile.Text = string.Empty;
        }
    }
}
