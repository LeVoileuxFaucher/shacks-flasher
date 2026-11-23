using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Shacks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un port COM.");
                return;
            }

            string comPort = comboBox1.SelectedItem.ToString();
            string arduinoType = comboBox2.SelectedItem.ToString();

            // Mapping du type Arduino pour avrdude
            string mcu;

            switch (arduinoType)
            {
                case "Mega 2560":
                    mcu = "m2560";
                    break;
                case "Uno":
                    mcu = "m328p";
                    break;
                default:
                    mcu = "m2560";
                    break;
            }


            // Chemin par défaut du projet PlatformIO (modifiable)
            string projectPath = @"C:\Users\Tristan\OneDrive - Cégep Limoilou\Documents\PlatformIO\Projects\Code Mega";

            // Chemin du fichier HEX généré par PlatformIO
            string hexFile = Path.Combine(projectPath, @".pio\build\megaatmega2560\firmware.hex");

            // Si le fichier n'existe pas, laisser l'utilisateur le choisir
            if (!File.Exists(hexFile))
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Sélectionner le firmware (.hex)";
                    ofd.Filter = "Fichiers HEX (*.hex)|*.hex|Tous les fichiers (*.*)|*.*";
                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;
                    hexFile = ofd.FileName;
                }
            }

            // Chemin avrdude et conf (à adapter selon ton installation Arduino)
            string avrdudePath = @"C:\Program Files (x86)\Arduino\hardware\tools\avr\bin\avrdude.exe";
            string avrdudeConf = @"C:\Program Files (x86)\Arduino\hardware\tools\avr\etc\avrdude.conf";

            if (!File.Exists(avrdudePath) || !File.Exists(avrdudeConf))
            {
                MessageBox.Show(
                    "avrdude est introuvable.\n\nInstalle l'IDE Arduino ou modifie les chemins dans Form1.cs.",
                    "avrdude manquant",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            string arguments = $"-C \"{avrdudeConf}\" -v -p{mcu} -cwiring -P{comPort} -b115200 -D -Uflash:w:\"{hexFile}\":i";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = avrdudePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();

                    // Afficher les résultats dans un RichTextBox au lieu de MessageBox
                    richTextBox1.Clear();
                    richTextBox1.AppendText("Output:\n" + output + "\n\nErrors:\n" + error);
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("Erreur : " + ex.Message);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }

            if (ports.Length > 0)
                comboBox1.SelectedIndex = 0;

            comboBox2.Items.Clear();
            comboBox2.Items.Add("Mega 2560");
            comboBox2.Items.Add("Uno");
            comboBox2.SelectedIndex = 0;
        }

        private void comboBoxPorts(object sender, EventArgs e)
        {

        }

        private void comboBox1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
