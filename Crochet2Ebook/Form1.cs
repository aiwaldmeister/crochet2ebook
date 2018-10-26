using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Crochet2Ebook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string Dateiname = "";
        private string Bildtitel = "Unbenannt";
        private string latexfiles_path = "";
        private Bitmap Originalbild;
        private Bitmap Zoombild;
        private Bitmap Displaybild;
        private Bitmap Rasterbild;
        private bool DisplayRatioCorrection = true;
        private double RatioCorrFactor_option = 0.8;
        private double active_RatioCorrFactor = 0.8;
        private int additionalZoomfactor = 1;
        private bool imageisloaded = false;
        private int selectedLine = 1;
        private PdfDocument myPDF;
        private Dictionary<string, string> DictColorNames_DE = new Dictionary<string, string>();
        private Dictionary<string, string> DictColorNames_EN = new Dictionary<string, string>();
        private string String_ColorNameList_DE = "";
        private string String_ColorNameList_EN = "";



        //Klicks auf Buttons
        private void button_ToggleOptions_Click(object sender, EventArgs e)
        {
            if (splitContainer4.Panel2Collapsed)
            {
                splitContainer4.Panel2Collapsed = false;
                button_ToggleOptions.Text = "Optionen verbergen";
            }
            else
            {
                splitContainer4.Panel2Collapsed = true;
                button_ToggleOptions.Text = "Optionen anzeigen";
            }
        }

        private void button_ToggleLanguage_Click(object sender, EventArgs e)
        {
            //vor dem sprachwechsel die Farbnamen sichern
            savecolornamestoConfig();
            if (radioButton_deutsch.Checked)
            {
                radioButton_deutsch.Checked = false;
                radioButton_englisch.Checked = true;
                label1_Sprache.Text = "ENGLISCH";
            }
            else
            {
                radioButton_englisch.Checked = false;
                radioButton_deutsch.Checked = true;
                label1_Sprache.Text = "DEUTSCH";
            }
            //die Palette mit der neuen Sprache neu aufbauen
            generatePalettefromImage();
        }

        private void button_createStuff_Click(object sender, EventArgs e)
        {

            if (Bildtitel.Equals(""))
            {
                MessageBox.Show("Bitte einen Bildtitel eingeben","Fehler!",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            int unnamedcolors_count = 0;
            foreach (ListViewItem p in listView_Palette.Items)
            {
                if (p.Text.Contains("#"))
                {
                    unnamedcolors_count++;
                }
            }
            if (unnamedcolors_count > 0)
            {
                DialogResult result = MessageBox.Show(unnamedcolors_count + " Farben der Farbpalette haben noch keinen sprechenden Namen.\n\nWirklich fortfahren?", "Achtung!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            
            //Warnen falls Titel noch Ziffern enthaelt
            if (Bildtitel.Any(char.IsDigit))
            {
                DialogResult result = MessageBox.Show("Der Titel '" + Bildtitel + "' enthält Ziffern.\n\nWirklich mit diesem Titel fortfahren?", "Achtung!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }



            System.IO.Directory.CreateDirectory(Bildtitel + "_Dateien");

            //PDF generieren
            if (checkBox_pdfvorlage.Checked)
            {
                createPDF();
            }

            //Rasterbild generieren
            createImagefiles();
            

            //Maschen und Farbwechsel ermitteln
            countMaschenUndFarbwechsel(Originalbild);

            //Textdateien generieren (infofile und LaTeX-dateien)
            createTextfiles();


            //Verzeichnis fuer die Palettenbilder erstellen
            System.IO.Directory.CreateDirectory(Bildtitel + "_Dateien/Palette");
            //Farbbilder abspeichern
            foreach (ListViewItem item in listView_Palette.Items)
            {
                String farbname = item.SubItems[0].Text;
                String Farbbildkey = item.SubItems[1].Text;
                String filename = Bildtitel + "_Dateien/Palette/" + entferneUmlautefuerDateinamen(farbname) + ".png";
                imageList_Palette.Images[Farbbildkey].Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            }


            //Verzeichnis mit Ergebnissen öffnen
            Process.Start("explorer.exe", Bildtitel + "_Dateien");
            
            //alle Generierungen beendet... alles zurück auf Anfang...
            progressBar1.Visible = false;

            numericUpDown_Zeile.Value = 1;
            selectedLine = Originalbild.Height - (int)numericUpDown_Zeile.Value;
            Zeile_Auswerten(selectedLine);

        }
        
        private void button_Palette_generatefromImage_Click(object sender, EventArgs e)
        {
            generatePalettefromImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button_LoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog_Picture.ShowDialog() == DialogResult.OK)
            {

                Dateiname = openFileDialog_Picture.FileName;
                ladeBild();

                textBox_Titel.Text = System.IO.Path.GetFileNameWithoutExtension(Dateiname);
                Bildtitel = textBox_Titel.Text;
                this.Text = Bildtitel + " - Pixelcounter 2.0";

                numericUpDown_Zeile.Value = 1;
                selectedLine = Originalbild.Height - (int)numericUpDown_Zeile.Value;
                Zeile_Auswerten(selectedLine);
                
            }
        }

        private void button_Zoom_Click(object sender, EventArgs e)
        {
            toggleZoom();
        }        

        
        //Durch sonstige interaktion mit Oberfläche ausgelöste Events
        private void checkBox_Ratiocorrection_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Ratiocorrection.Checked)
            {
                DisplayRatioCorrection = true;
            }
            else
            {
                DisplayRatioCorrection = false;
            }

            if (imageisloaded)
            {
                generateZoomedImage();
                Displaybild = Zoombild;
                fitImagetoFrame();
                
                Zeilemarkieren(selectedLine);
                refreshDisplay();
            }
        }

        private void textBox_Titel_TextChanged(object sender, EventArgs e)
        {
            Bildtitel = textBox_Titel.Text;
            this.Text = Bildtitel + " - Pixelcounter 2.0";
        }

        private void pictureBox_Display_MouseClick(object sender, MouseEventArgs e)
        {   if(e.Button == MouseButtons.Left)
            {
                numericUpDown_Zeile.Value = Originalbild.Height - (int)((double)e.Y / getZoomFactorY());
                textBox_Beispielreihe1.Text = numericUpDown_Zeile.Value.ToString();
                textBox_Beispielreihe2.Text = (numericUpDown_Zeile.Value +1 ).ToString();
                selectedLine = Originalbild.Height - (int)numericUpDown_Zeile.Value;
                Zeile_Auswerten(selectedLine);
            }
        }

        private void splitContainer4_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox_Ratokorrfaktor_TextChanged(object sender, EventArgs e)
        {
            double faktor = RatioCorrFactor_option;
            if (double.TryParse(textBox_Ratokorrfaktor.Text, out faktor))
            {
                if(faktor>0)
                {
                    RatioCorrFactor_option = faktor;
                    if (imageisloaded)
                    {
                        generateZoomedImage();
                        Displaybild = Zoombild;
                        fitImagetoFrame();

                        Zeilemarkieren(selectedLine);
                        refreshDisplay();
                    }
                }

            }

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //additionalZoomfactor = 1;
            if (imageisloaded)
            {
                generateZoomedImage();
                fitImagetoFrame();
                Zeilemarkieren(selectedLine);
            }
            
        }
        
        private void numericUpDown_Zeile_ValueChanged(object sender, EventArgs e)
        {
            selectedLine = Originalbild.Height - (int)numericUpDown_Zeile.Value;
            Zeile_Auswerten(selectedLine);
        }

        private void listView_LineDescription_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Zeilemarkieren(selectedLine);

        }

        private void listView_Palette_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo HI = listView_Palette.HitTest(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (HI.Location == ListViewHitTestLocations.None)
                {   //wurde kein item getroffen, werden optionen ausgeblendet
                    contextMenu_Palette.Items[0].Visible = false;
                }
                else
                {   //wurde ein item getroffen, werden optionen eingeblendet
                    contextMenu_Palette.Items[0].Visible = true;
                }
                contextMenu_Palette.Show(Cursor.Position);
            }
        }

        private void dieseFarbeUmbenennenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView_Palette.FocusedItem.BeginEdit();
        }

        private void listView_Palette_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.F2)
            {
                listView_Palette.FocusedItem.BeginEdit();
            }
        }

        private void checkBox_AutoRasterfarbe_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_AutoRasterfarbe.Checked)
            {
                textBox_Rasterbild_Linienfarbe.Enabled = false;
            }
            else
            {
                textBox_Rasterbild_Linienfarbe.Enabled = true;
            }

        }

        private void checkBox_AutoBeispielreihen_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_AutoBeispielreihen.Checked)
            {
                textBox_Beispielreihe1.Enabled = false;
                textBox_Beispielreihe2.Enabled = false;
            }
            else
            {
                textBox_Beispielreihe1.Enabled = true;
                textBox_Beispielreihe2.Enabled = true;
            }
        }
        

        //Start und Ende des Programms
        private void Form1_Load(object sender, EventArgs e)
        {
            listView_LineDescription.Clear();

            string iniDateiname = GetSetting("Image");
            string iniLine = GetSetting("Line");
            string iniTitle = GetSetting("Title");
            string iniDisplayRatioCorrection = GetSetting("DisplayRatioCorrection");
            string iniColorNameList_DE = GetSetting("ColorNameList_DE");
            string iniColorNameList_EN = GetSetting("ColorNameList_EN");
            bool deu = radioButton_deutsch.Checked;
            bool eng = radioButton_englisch.Checked;

            //den Pfad der allgemein gehaltenen LaTex-Files aus der Config holen...
            latexfiles_path = GetSetting("latexfiles");
            if (latexfiles_path == "")
            {
                MessageBox.Show("Bitte in der Config den Pfad zu den allgemeinen LaTex-Files unter dem Key 'latexfiles' nachtragen...");
            }

            //Rasterfarbe aus der ini holen
            //textBox_Rasterbild_Linienfarbe.Text = GetSetting("Rasterfarbe");


            int iniLineint = 1;
            int.TryParse(iniLine, out iniLineint);
            
            splitContainer4.Panel2Collapsed = true;

            //Imagelist leeren
            imageList_Palette.Images.Clear();




            //initialisiere die Dictionary mit den Farbnamen aus dem String iniColorNameList_DE
            if (iniColorNameList_DE == null)
            {
                iniColorNameList_DE = "";
            }

            string[] iniFarbstrings_DE = iniColorNameList_DE.Split(';');
            foreach (String Farbstring in iniFarbstrings_DE)
            {
                if (!Farbstring.Equals(""))
                {
                    DictColorNames_DE.Add(Farbstring.Substring(0,7),Farbstring.Substring(8));
                }
            }


            //initialisiere die Dictionary mit den Farbnamen aus dem String iniColorNameList_DE
            if (iniColorNameList_EN == null)
            {
                iniColorNameList_EN = "";
            }

            string[] iniFarbstrings_EN = iniColorNameList_EN.Split(';');
            foreach (String Farbstring in iniFarbstrings_EN)
            {
                if (!Farbstring.Equals(""))
                {
                    DictColorNames_EN.Add(Farbstring.Substring(0, 7), Farbstring.Substring(8));
                }
            }



            if (System.IO.File.Exists(iniDateiname))
            {
                Dateiname = iniDateiname;
                textBox_Titel.Text = iniTitle;
                Bildtitel = iniTitle;
                this.Text = Bildtitel + " - Pixelcounter 2.0";

                if (iniDisplayRatioCorrection.Equals("0"))
                {
                    checkBox_Ratiocorrection.Checked = false;
                    DisplayRatioCorrection = false;
                }else{
                    checkBox_Ratiocorrection.Checked = true;
                    DisplayRatioCorrection = true;
                }

                ladeBild();
                numericUpDown_Zeile.Value = iniLineint;
                selectedLine = Originalbild.Height - (int)numericUpDown_Zeile.Value;
                Zeile_Auswerten(selectedLine);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Status, Settings und Farbnamen in die Config schreiben
            SetSetting("Image", Dateiname);
            SetSetting("Title", Bildtitel);
            SetSetting("Line",numericUpDown_Zeile.Value.ToString());
            if (DisplayRatioCorrection)
            {
                SetSetting("DisplayRatioCorrection", "1");
            }else{
                SetSetting("DisplayRatioCorrection", "0");
            }
            savecolornamestoConfig();

            //Rasterfarbe in der ini speichern
            //SetSetting("Rasterfarbe", textBox_Rasterbild_Linienfarbe.Text);

        }


        //Farbpalette
        private void generatePalettefromImage()
        {
            List<Color> Farbliste;
            Farbliste = scanImageforColors(Originalbild);

            String colorCode = "";
            listView_Palette.Items.Clear(); //Palette leeren...
            foreach (Color Col in Farbliste)  //...und neu befuellen
            {
                colorCode = ColortoHex(Col);
                addNewColor(colorCode);
            }
        }

        private List<Color> scanImageforColors(Bitmap Bild)
        {
            List<Color> genPalette = new List<Color>();

            bool farbebereitsvorhanden = false;
            Color pixelfarbe;

            progressBar1.Maximum = Bild.Width;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            try
            {
                for (int x = 0; x < Bild.Size.Width; x++)
                {
                    progressBar1.Value = x;
                    for (int y = 0; y < Bild.Size.Height; y++)
                    {
                        //Falls bereits 100 Farben gefunden wurden, Benutzer warnen und ggf. abbrechen.
                        if(genPalette.Count >= 1000)
                        {
                            DialogResult res = MessageBox.Show("Es wurden schon 1000 verschiedene Farben gefunden!\nWirklich weitermachen?","Sehr viele Farben!",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                            if (res == DialogResult.No)
                            {
                            return genPalette;
                            }
                        }

                        pixelfarbe = Bild.GetPixel(x, y);
                        farbebereitsvorhanden = false;
                        foreach (Color listfarbe in genPalette)
                        {
                            if (listfarbe == pixelfarbe)
                            {
                                farbebereitsvorhanden = true;
                            }
                        }
                        if (!farbebereitsvorhanden)
                        {
                            genPalette.Add(pixelfarbe);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception: " + e);
            }
            progressBar1.Visible = false;
            return genPalette;

        }

        private void addNewColor(String colorCode)
        {
            bool deu = radioButton_deutsch.Checked;
            bool eng = radioButton_englisch.Checked;
            
            String colorName = colorCode;

            if (deu)
            {
                //Namen für die Farbe finden
                if (DictColorNames_DE.Keys.Contains(colorCode))
                {
                    colorName = DictColorNames_DE[colorCode];
                }
            }
            if (eng)
            {
                //Namen für die Farbe finden
                if (DictColorNames_EN.Keys.Contains(colorCode))
                {
                    colorName = DictColorNames_EN[colorCode];
                }
            }
            

            ListViewItem item = listView_Palette.Items.Add(colorName);
            item.SubItems.Add(colorCode);
            item.SubItems.Add(0.ToString());   //für die Maschenzahl dieser Farbe
            item.SubItems.Add(0.ToString());   //für die Anzahl Farbanfaenge mit dieser Farbe
            item.SubItems.Add(0.ToString());   //für die Anzahl Farbenden mit dieser Farbe


            //neues Bild für die Farbvorschau in die Imagelist aufnehmen...
            imageList_Palette.Images.Add(colorCode, generateColorPreviewImage(HextoColor(colorCode)));
            item.ImageKey = colorCode;  //...und ans item haengen
            listView_Palette.ShowItemToolTips = true;
            item.ToolTipText = colorCode;
        }

        private Image generateColorPreviewImage(Color Farbe)
        {
            Bitmap bmp = new Bitmap(64, 64);
            Pen pen = new Pen(Color.Black, 4);
            Graphics G = Graphics.FromImage(bmp);
            SolidBrush brush = new SolidBrush(Farbe);
            G.FillRectangle(brush, 0, 0, 64, 64);
            G.DrawRectangle(pen, 0, 0, 64, 64);
            return bmp;
        }

        private void sortierePalettenachLauflaengen()
        {
            float toplauflaenge = 0;
            int topindex = 0;
            ListViewItem tmpitem = null;

            for (int i = 0; i < listView_Palette.Items.Count; i++)
            {
                toplauflaenge = 0;
                for (int k = 0; k < listView_Palette.Items.Count - i; k++)
                {
                    float thislauflaenge = 0;
                    float.TryParse(listView_Palette.Items[k].SubItems[4].Text, out thislauflaenge);
                    if (thislauflaenge > toplauflaenge)
                    {
                        toplauflaenge = thislauflaenge;
                        topindex = k;
                    }
                }
                tmpitem = listView_Palette.Items[topindex];
                listView_Palette.Items.RemoveAt(topindex);
                listView_Palette.Items.Add(tmpitem);
            }
        }

        
        //Anzeige
        private void refreshDisplay()
        {
              
              pictureBox_Display.Image = Displaybild;
//            pictureBox_Display.Refresh();
//            pictureBox_Display.Update();
        }

        private void fitImagetoFrame()
        {
            if (imageisloaded)
            {
                //generateZoomedImage();

                //Picturebox auf Größe des Bildes bringen
                pictureBox_Display.Size = Displaybild.Size;

                //Zoomfaktor auf die Picturebox anwenden
                pictureBox_Display.Height = pictureBox_Display.Height * additionalZoomfactor;
                pictureBox_Display.Width = pictureBox_Display.Width * additionalZoomfactor;

                //PictureBox mittig im Panel platzieren
                int PosX = 0;
                int PosY = 0;
                if(pictureBox_Display.Width < splitContainer1.Panel2.Width)
                {
                    PosX = (int)(0.5 * (splitContainer1.Panel2.Width - pictureBox_Display.Width));
                }

                if (pictureBox_Display.Height < splitContainer1.Panel2.Height)
                {
                    PosY = (int)(0.5 * (splitContainer1.Panel2.Height - pictureBox_Display.Height));
                }
    

                Displaybild = Zoombild;
                refreshDisplay();

                Point myLocation = new Point(PosX,PosY);
                pictureBox_Display.Location = myLocation;
            }
        }

        private void toggleZoom()
        {
            if(additionalZoomfactor < 8)
            {
                additionalZoomfactor = additionalZoomfactor * 2;
            }
            else
            {
                additionalZoomfactor = 1;
            }
            
            generateZoomedImage();
            fitImagetoFrame();
            Displaybild = Zoombild;
            Zeilemarkieren(selectedLine);
            refreshDisplay();
        }

        private void generateZoomedImage()
        {
            if (DisplayRatioCorrection)
            {
                active_RatioCorrFactor = RatioCorrFactor_option;
            }
            else
            {
                active_RatioCorrFactor = 1;
            }
            int newWidth, newHeight;

            int maxWidth = splitContainer1.Panel2.Width-2;
            int maxHeight = splitContainer1.Panel2.Height-2;

            if (((double)Originalbild.Width * active_RatioCorrFactor) / ((double)Originalbild.Height) > (double)maxWidth / (double)maxHeight)
            {//Bild ist (mit korrekturfaktor) breiter als hoch... breite auf breite der Picturebox setzen, höhe verhältnis
                newWidth = maxWidth;
                newHeight = (int)((((double)maxWidth / (double)Originalbild.Width) * (double)Originalbild.Height) / active_RatioCorrFactor);
            }
            else
            {//Bild ist (mit korrekturfaktor) hoeher als breit... hoehe auf hoehe der Picturebox setzen, breite im verhältnis
                newHeight = (int)((double)maxHeight);
                newWidth = (int)((((double)maxHeight / (double)Originalbild.Height) * (double)Originalbild.Width) * active_RatioCorrFactor);
            }

            //newHeight = newHeight * additionalZoomfactor;
            //newWidth = newWidth * additionalZoomfactor;
            try
            {
                Zoombild = new Bitmap(newWidth, newHeight);

            }
            catch (Exception)
            {
                
            }
            using (Graphics gr = Graphics.FromImage(Zoombild))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                gr.DrawImage(Originalbild, new Rectangle(0, 0, newWidth, newHeight));
            }
        }

        private void Zeilemarkieren(int zumarkierendeZeile)
        {

            Displaybild = Zoombild;
            
            float ZoomFactorY = getZoomFactorY();

            Rectangle markierung = new Rectangle(0, (int)(Math.Round(zumarkierendeZeile*ZoomFactorY)), Displaybild.Width-1, (int)(Math.Round(ZoomFactorY)));
            Color markierungsfarbe1 = Color.Black;
            Color markierungsfarbe2 = Color.White;
            
            int Stiftdicke = 1;
            if (ZoomFactorY > 8) { Stiftdicke = 2; }
            if (ZoomFactorY > 12) { Stiftdicke = 3; }
            
            Pen mypen1 = new Pen(markierungsfarbe1, Stiftdicke);
            Pen mypen2 = new Pen(markierungsfarbe2, Stiftdicke);
            mypen2.DashPattern = new float[] { 2, 2 };

            using (Graphics grD = Graphics.FromImage(Displaybild))
            {
                grD.DrawRectangle(mypen1, markierung);
                grD.DrawRectangle(mypen2, markierung);

                grD.Dispose();
            } 

            //workaround damit die alte Markierung weggeht ohne ein neues image zu erzeugen;
            generateZoomedImage();
            refreshDisplay();
            

            Farbbereichmarkieren(zumarkierendeZeile);
        }

        private void Farbbereichmarkieren(int zumarkierendeZeile)
        {

            foreach (ListViewItem selection in listView_LineDescription.SelectedItems)
            {
               
                Displaybild = Zoombild;


                int startpixel = 0;
                int pixelanzahl = 0;

                int.TryParse(selection.SubItems[5].Text, out startpixel);
                int.TryParse(selection.SubItems[6].Text, out pixelanzahl);

                float ZoomFactorX = getZoomFactorX();
                float ZoomFactorY = getZoomFactorY();

                Rectangle markierung = new Rectangle((int)Math.Round(((startpixel -1) * ZoomFactorX)), (int)Math.Round((zumarkierendeZeile * ZoomFactorY)), (int)Math.Round((pixelanzahl * ZoomFactorX)), (int)Math.Round((ZoomFactorY)));
                Color markierungsfarbe1 = Color.Black;
                Color markierungsfarbe2 = Color.White;

                int Stiftdicke = 1;
                if (ZoomFactorY > 8) { Stiftdicke = 2; }
                if (ZoomFactorY > 12) { Stiftdicke = 3; }

                Pen mypen1 = new Pen(markierungsfarbe1, Stiftdicke);
                Pen mypen2 = new Pen(markierungsfarbe2, Stiftdicke);
                mypen2.DashPattern = new float[] { 2, 2 };

                using (Graphics grD = Graphics.FromImage(Displaybild))
                {
                    grD.DrawRectangle(mypen1, markierung);
                    grD.DrawRectangle(mypen2, markierung);

                    grD.Dispose();
                }
                
            }

            //workaround damit die alte Markierung weggeht ohne ein neues image zu erzeugen;
            generateZoomedImage();
            refreshDisplay();
        }


        //Hilfsfunktionen
        private void ladeBild()
        {
                Originalbild = new Bitmap(Dateiname);
                generateZoomedImage();
                Displaybild = Zoombild;
                generatePalettefromImage();
                refreshDisplay();
                imageisloaded = true;
                pictureBox_Display.Visible = true;
                fitImagetoFrame();
                
                listView_LineDescription.Enabled = true;
                numericUpDown_Zeile.Enabled = true;
                button_Zoom.Enabled = true;
                button_create_stuff.Enabled = true;
                
                numericUpDown_Zeile.Maximum = Originalbild.Height;
                
        }

        private string Zeile_Auswerten(int Zeile)
        {
            string LineDescription = "";
            listView_LineDescription.Clear();
           
            Zeilemarkieren(Zeile);
            Color laufendeFarbe = Originalbild.GetPixel(Originalbild.Width - 1, Zeile);
            Color pixelfarbe = laufendeFarbe;
            int pixelcounter = 0;
            int startpixel = 0;
            int endpixel = Originalbild.Width;


 
            for (int i = Originalbild.Width; i >= 1; i--)
            {
                startpixel = i+1; //noch für die laufende Farbe
                pixelfarbe = Originalbild.GetPixel(i - 1, Zeile);
                if (pixelfarbe == laufendeFarbe)
                {
                    //pixelzähler erhöhen
                    pixelcounter++;
                }
                else
                {   
                    
                    //Ausgeben der beendeten Farbe
                    LineDescription = LineDescription + getLaufendeFarbe(pixelcounter, laufendeFarbe) + " ";
                    AddItemtoLineDescription(pixelcounter, startpixel, endpixel, laufendeFarbe);
                    laufendeFarbe = pixelfarbe;
                    
                    //nächste Farbe anfangen
                    pixelcounter = 1;
                    endpixel = i; //schon für die neue Farbe
                }
            }
            //Ausgeben der letzten Farbe
            startpixel--;
            LineDescription = LineDescription + getLaufendeFarbe(pixelcounter, laufendeFarbe);
            AddItemtoLineDescription(pixelcounter, startpixel, endpixel, laufendeFarbe);

            textBox1.Text = LineDescription;
            return LineDescription;
        }

        private void AddItemtoLineDescription(int pixelanzahl, int startpixel, int endpixel, Color laufendeFarbe)
        {
            foreach (ListViewItem PaletteItem in listView_Palette.Items)
            {
                if (HextoColor(PaletteItem.SubItems[1].Text) == laufendeFarbe)
                {
                    var myNewItem = listView_LineDescription.Items.Add((ListViewItem)PaletteItem.Clone());
                    myNewItem.ImageIndex = PaletteItem.ImageIndex;
                    myNewItem.ImageKey = PaletteItem.ImageKey;
                    string Farbname = myNewItem.Text;

                    myNewItem.Text = " " + pixelanzahl.ToString(); // + " " + myNewItem.Text;

                    myNewItem.SubItems.Add(startpixel.ToString());
                    myNewItem.SubItems.Add(pixelanzahl.ToString());
                    myNewItem.SubItems.Add(Farbname);


                    myNewItem.Font = listView_LineDescription.Font;

                    break;
                }

            }
        }

        private void selectLine(int v)
        {
            selectedLine = v;
        }

        private float getZoomFactorX()
        {
            return (float)Zoombild.Width / (float)Originalbild.Width;
        }

        private float getZoomFactorY()
        {
            return (float)Zoombild.Height / (float)Originalbild.Height;
        }

        private void countMaschenUndFarbwechsel(Bitmap Bild)
        {
            Color pixelfarbe_current;
            Color pixelfarbe_last;

            const int Maschenanzahl = 2;
            const int Farbanfang = 3;
            const int Farbende = 4;

            //Die Zähler für jede Farbe leeren
            foreach (ListViewItem item in listView_Palette.Items)
            {
                item.SubItems[2].Text = 0.ToString();
                item.SubItems[3].Text = 0.ToString();
                item.SubItems[4].Text = 0.ToString();
            }


            pixelfarbe_last = Bild.GetPixel(0, 0);
            //einen Farbanfang für die erste Farbe zählen
            increaseSubitemCounter(pixelfarbe_last, Farbanfang);
            


            //Maschen je Farbe zählen
            for (int y = 0; y < Bild.Size.Height; y++)
            {
                for (int x = 0; x < Bild.Size.Width; x++)
                {
                    pixelfarbe_current = Bild.GetPixel(x, y);

                    //Maschencounter für Farbe hochzählen
                    increaseSubitemCounter(pixelfarbe_current, Maschenanzahl);

                    //Mit voriger Farbe vergleichen und ggf. beide Farbwechselcounter hochzählen (anfangend für aktuelle Farbe, endend für letzte Farbe)
                    if (pixelfarbe_current!=pixelfarbe_last)
                    {
                        increaseSubitemCounter(pixelfarbe_current, Farbanfang);
                        
                    }
                    pixelfarbe_last = pixelfarbe_current;
                }
            }

            increaseSubitemCounter(pixelfarbe_last, Farbende);

            //Farbliste nach Maschenzahlen absteigend sortieren
            berechneLauflaengen();
            sortierePalettenachLauflaengen();

            

        }

        private void berechneLauflaengen()
        {
            //Lauflaengen_Werte aus den Textboxen holen
            float Lauflaenge_Masche = 0;
            float Lauflaenge_Wechsel = 0;
            float.TryParse(textBox_Lauflaenge_Masche.Text, out Lauflaenge_Masche);
            float.TryParse(textBox_Lauflaenge_Wechsel.Text, out Lauflaenge_Wechsel);
            foreach (ListViewItem item in listView_Palette.Items)
            {
                int Maschenzahl_DieseFarbe = 0;
                int Wechsel_DieseFarbe = 0;
                Int32.TryParse(item.SubItems[2].Text, out Maschenzahl_DieseFarbe);
                Int32.TryParse(item.SubItems[3].Text, out Wechsel_DieseFarbe);
                item.SubItems[4].Text = ((Lauflaenge_Masche * Maschenzahl_DieseFarbe) + (Lauflaenge_Wechsel * Wechsel_DieseFarbe)).ToString();
            }
        }

        private void increaseSubitemCounter(Color Farbe, int Index)
        {
            foreach (ListViewItem item in listView_Palette.Items)
            {
                if (item.ToolTipText == ColortoHex(Farbe))
                {
                    int count = 0;
                    Int32.TryParse(item.SubItems[Index].Text, out count);
                    count++;
                    item.SubItems[Index].Text = count.ToString();
                }
            }
        }

        private string getLaufendeFarbe(int pixelcounter, Color laufendeFarbe)
        {
            string Farbname = "";
            //Farbname aus der Palette ermitteln
            foreach (ListViewItem item in listView_Palette.Items)
            {
                if (HextoColor(item.SubItems[1].Text) == laufendeFarbe)
                {
                    Farbname = item.Text;
                    
                    break;
                }
                
            }

            return pixelcounter + "_" + Farbname;
        }

        private string entferneUmlautefuerLaTex(string sourceText)
        {
            string targetText = sourceText;

            targetText = targetText.Replace("ß", "{\\ss}");
            targetText = targetText.Replace("Ä", "\"A");
            targetText = targetText.Replace("ä", "\"a");
            targetText = targetText.Replace("Ö", "\"O");
            targetText = targetText.Replace("ö", "\"o");
            targetText = targetText.Replace("Ü", "\"U");
            targetText = targetText.Replace("ü", "\"u");
            targetText = targetText.Replace("#", "\\#");

            return targetText;
        }

        private string entferneUmlautefuerDateinamen(string sourceText)
        {
            string targetText = sourceText;

            targetText = targetText.Replace("ß", "ss");
            targetText = targetText.Replace("Ä", "Ae");
            targetText = targetText.Replace("ä", "ae");
            targetText = targetText.Replace("Ö", "Oe");
            targetText = targetText.Replace("ö", "oe");
            targetText = targetText.Replace("Ü", "Ue");
            targetText = targetText.Replace("ü", "ue");
            targetText = targetText.Replace(" ", "_");
            targetText = targetText.Replace("#", "");

            return targetText;
        }

        public static String ColortoHex(Color Col)
        {
            return ColorTranslator.ToHtml(Col);
        }

        public static Color HextoColor(string colorCode)
        {
            return System.Drawing.ColorTranslator.FromHtml(colorCode);
        }

        private void FindeIdealeRasterfarbe()
        {
            float Minimaler_Farbabstand_ThisTry = float.MaxValue;
            float Minimaler_Farbabstand_BestTry = 0;
            String ColorCode_IdealeRasterfarbe = "";
            String ColorCode_Thistry = "";

            //Es werden alle 256 moeglichen Graustufen-Werte als potentielle Rasterfarbe durchgetestet
            for (int i = 0; i < 256; i++)
            {
                //Farbcode fuer die zu testende Farbe bilden...
                ColorCode_Thistry = "#" + i.ToString("X2") + i.ToString("X2") + i.ToString("X2");

                //Referenzwert wieder zurueck nach ganz oben setzen...
                Minimaler_Farbabstand_ThisTry = float.MaxValue;

                //fuer jeden Versuch wird mit allen Palettenfarben verglichen
                foreach (ListViewItem item in listView_Palette.Items)
                {
                    //Farbe des Palettenitems holen...
                    String Colorcode_ThisPaletteItem = item.SubItems[1].Text;

                    //Die beiden Farben vergleichen und abstand abschätzen
                    float Dieser_Farbabstand = Math.Abs(getBrightness(Colorcode_ThisPaletteItem) - getBrightness(ColorCode_Thistry));

                    if (Dieser_Farbabstand < Minimaler_Farbabstand_ThisTry)
                    {
                        //neuer niedrigster Farbabstand fuer diesen Versuch
                        Minimaler_Farbabstand_ThisTry = Dieser_Farbabstand;
                    }
                }

                if (Minimaler_Farbabstand_ThisTry > Minimaler_Farbabstand_BestTry)
                {
                    //Diese Farbe hat einen hoeheren geringsten Farbabstand -> Besser -> also bisher bester Try
                    Minimaler_Farbabstand_BestTry = Minimaler_Farbabstand_ThisTry;
                    ColorCode_IdealeRasterfarbe = ColorCode_Thistry;
                }
                
            }

            //die hoffentlich Ideale Farbe ist gefunden... -> in die Textbox eintragen
            textBox_Rasterbild_Linienfarbe.Text = ColorCode_IdealeRasterfarbe;

        }

        public static float getBrightness(string colorCode)
        {
            Color c = HextoColor(colorCode);

            //Formel die die Helligkeit für RGB-Farben abschaetzt.
            //Die RGB -Anteile werden dazu verschieden gewichtet
            return (float)Math.Sqrt(
                c.R * c.R * .241 +
                c.G * c.G * .691 +
                c.B * c.B * .068);
        }

        
        //Dateien erzeugen
        private void createTextfiles()
        {
            float Breite_Masche = 0;
            float Hoehe_Masche = 0;
            string inhalt_Infofile = "";
            string file_langsuffix = "";

            bool eng = radioButton_englisch.Checked;
            bool deu = radioButton_deutsch.Checked;
            if (eng)
            {
                file_langsuffix = "_eng";
            }

            string inhalt_texfile_Main = "";
            string inhalt_projektfile = "";
            string inhalt_texfile_titelseite = "";
            string inhalt_texfile_wollmengen = "";
            string inhalt_texfile_beispielreihen = "";
            string name_texfile_titelseite = Bildtitel + "_Dateien/Latex_Dateien/" + "titelseite" + file_langsuffix + ".tex";
            string name_texfile_wollmengen = Bildtitel + "_Dateien/Latex_Dateien/" + "wollmengen" + file_langsuffix + ".tex";
            string name_texfile_beispielreihen = Bildtitel + "_Dateien/Latex_Dateien/" + "beispielreihen" + file_langsuffix + ".tex";
            string name_texfile_Main = "";
            string name_projektfile = "";
            string ProjektTitleString = "";

            //Beispielzeilen ermitteln
            if (checkBox_AutoBeispielreihen.Checked)
            {
                for (int i = Originalbild.Height-1; i >= 0; i--)
                {
                    Zeile_Auswerten(i);
                    if (listView_LineDescription.Items.Count>1)
                    {
                        textBox_Beispielreihe1.Text = (Originalbild.Height - i).ToString();
                        textBox_Beispielreihe2.Text = (Originalbild.Height - i + 1).ToString();
                        break;
                    }
                }
            }


            if (deu)
            {
                ProjektTitleString = "Anleitung_Kinderdecke_-_";
            }
            if (eng)
            {
                ProjektTitleString = "Tutorial_Childrens_Blanket_-_";
            }


            name_texfile_Main = Bildtitel + "_Dateien/Latex_Dateien/" + ProjektTitleString + Bildtitel + ".tex";
            name_projektfile = Bildtitel + "_Dateien/" + ProjektTitleString + Bildtitel + ".tcp";
                        
            string LauflaengenString = "Lauflängen ca.:\r\n----------------\r\n";
            string MaschenzahlenString = "Maschenanzahl:\r\n--------------\r\n";
            string GroessenString = "(" + Originalbild.Width + " x " + Originalbild.Height + " Maschen / ca. ";
            string IntroString = "Häkeldecke '" + Bildtitel + "'\r\n";

            //Verzeichnis fuer die LaTex Dateien erstellen
            System.IO.Directory.CreateDirectory(Bildtitel + "_Dateien/Latex_Dateien");
            //die allgemeinen texfiles vom latexfile-path in den Ordner dieses Projekts kopieren...
            System.IO.File.Copy(latexfiles_path + "anleitung_decke_annaehen_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "anleitung_decke_annaehen_allgemein" + file_langsuffix + ".tex", true);
            System.IO.File.Copy(latexfiles_path + "anleitung_motiv_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "anleitung_motiv_allgemein" + file_langsuffix + ".tex", true);
            System.IO.File.Copy(latexfiles_path + "disclaimer_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "disclaimer_allgemein" + file_langsuffix + ".tex", true);
            System.IO.File.Copy(latexfiles_path + "schlusstext_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "schlusstext_allgemein" + file_langsuffix + ".tex", true);
            System.IO.File.Copy(latexfiles_path + "werkzeug_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "werkzeug_allgemein" + file_langsuffix + ".tex", true);
            System.IO.File.Copy(latexfiles_path + "struktur_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "struktur_allgemein" + file_langsuffix + ".tex", true);
            System.IO.File.Copy(latexfiles_path + "rasterbild_allgemein" + file_langsuffix + ".tex", Bildtitel + "_Dateien/Latex_Dateien/" + "rasterbild_allgemein" + file_langsuffix + ".tex", true);

            //Verzeichnis fuer die Anleitungsfotos Dateien erstellen
            System.IO.Directory.CreateDirectory(Bildtitel + "_Dateien/Fotos");
            //die Anleitungsfotos vom latexfile-path in den Ordner dieses Projekts kopieren...
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_farbwechsel_01.png", Bildtitel + "_Dateien/Fotos/tunesisch_farbwechsel_01.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_farbwechsel_02.png", Bildtitel + "_Dateien/Fotos/tunesisch_farbwechsel_02.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_farbwechsel_03.png", Bildtitel + "_Dateien/Fotos/tunesisch_farbwechsel_03.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_farbwechsel_04.png", Bildtitel + "_Dateien/Fotos/tunesisch_farbwechsel_04.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_farbwechsel_05.png", Bildtitel + "_Dateien/Fotos/tunesisch_farbwechsel_05.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_farbwechsel_07.png", Bildtitel + "_Dateien/Fotos/tunesisch_farbwechsel_07.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_hinrunde_01.png", Bildtitel + "_Dateien/Fotos/tunesisch_hinrunde_01.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_hinrunde_02.png", Bildtitel + "_Dateien/Fotos/tunesisch_hinrunde_02.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_rueckrunde_01.png", Bildtitel + "_Dateien/Fotos/tunesisch_rueckrunde_01.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_rueckrunde_02.png", Bildtitel + "_Dateien/Fotos/tunesisch_rueckrunde_02.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_rueckrunde_03.png", Bildtitel + "_Dateien/Fotos/tunesisch_rueckrunde_03.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/tunesisch_rueckrunde_04.png", Bildtitel + "_Dateien/Fotos/tunesisch_rueckrunde_04.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/decke_annaehen_01.png", Bildtitel + "_Dateien/Fotos/decke_annaehen_01.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/decke_annaehen_02.png", Bildtitel + "_Dateien/Fotos/decke_annaehen_02.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/decke_annaehen_03.png", Bildtitel + "_Dateien/Fotos/decke_annaehen_03.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/decke_annaehen_04.png", Bildtitel + "_Dateien/Fotos/decke_annaehen_04.png", true);
            System.IO.File.Copy(latexfiles_path + "Fotos/logo_wollmaus_01.png", Bildtitel + "_Dateien/Fotos/logo_wollmaus_01.png", true);


            inhalt_projektfile = 
                "[FormatInfo]\r\n"+
                "Type=TeXnicCenterProjectInformation\r\n"+
                "Version=4\r\n"+
                "\r\n"+
                "[ProjectInfo]\r\n"+
                "MainFile=Latex_Dateien/" + ProjektTitleString + Bildtitel + ".tex\r\n"+
                "UseBibTeX=0\r\n"+
                "UseMakeIndex=0\r\n"+
                "ActiveProfile=LaTeX ⇨ PDF\r\n"+
                "ProjectLanguage=de\r\n"+
                "ProjectDialect=DE\r\n"+
                "\r\n";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(name_projektfile))
            {
                sw.Write(inhalt_projektfile);
            }


            if (deu)
            {
                inhalt_texfile_wollmengen = 
                    "\\\\\\\\Wieviel Wolle du von jeder Farbe genau brauchst h\"angt davon ab, wie locker bzw.fest du h\"akelst und wieviel Faden du bei den Farbwechseln stehen l\"asst. " + 
                    "Die folgenden Mengenangaben sind meine Erfahrungswerte und dienen nur zur groben Absch\"atzung.:\\\\\\\\\r\n";
            }
            if (eng)
            {
                inhalt_texfile_wollmengen =
                    "\\\\\\\\How much wool of each colour you really need depends on how tight or loose you crochet and how much slack you leave when colours change. " +
                    "The following numbers are estimates based on my experience to use as a rough guidance.:\\\\\\\\\r\n";
            }


            foreach (ListViewItem item in listView_Palette.Items)
            {
                MaschenzahlenString = MaschenzahlenString + item.Text + ": " + item.SubItems[2].Text + "\r\n";

                const double grammprometer = 0.4;
                float Lauflaenge_DieseFarbe = 0;
                float.TryParse(item.SubItems[4].Text, out Lauflaenge_DieseFarbe);
                
                string Lauflaenge_DieseFarbe_mitEinheit = "";
                string Gewicht_DieseFarbe_mitEinheit = "";
                string Farbname_DieseFarbe = "";

                Farbname_DieseFarbe = item.Text;
                Gewicht_DieseFarbe_mitEinheit = Math.Ceiling(Lauflaenge_DieseFarbe * 0.01 * grammprometer).ToString() + "g";

                if (Lauflaenge_DieseFarbe > 100)
                {
                    //als Meter ausgeben
                    Lauflaenge_DieseFarbe_mitEinheit = Math.Ceiling(Lauflaenge_DieseFarbe * 0.01).ToString() + "m";
                }
                else
                {
                    //als Zentimeter ausgeben
                    Lauflaenge_DieseFarbe_mitEinheit = Math.Ceiling(Lauflaenge_DieseFarbe).ToString() + "cm";
                }

                LauflaengenString = LauflaengenString + Farbname_DieseFarbe + ": " + Lauflaenge_DieseFarbe_mitEinheit +  " (~" + Gewicht_DieseFarbe_mitEinheit + ")\r\n";

                inhalt_texfile_wollmengen =
                    inhalt_texfile_wollmengen +
                    "\\begin{minipage}[c][17mm]{0.33\\linewidth}\r\n" +
                    "  \\begin{minipage}[c][16mm]{0.35\\linewidth}\r\n" +
                    "    \\centering\r\n" +
                    "    \\includegraphics[width=12mm]{../Palette/" + entferneUmlautefuerDateinamen(Farbname_DieseFarbe) + ".png}\r\n" +
                    "  \\end{minipage}\r\n" +
                    "  \\begin{minipage}[c][16mm]{0.63\\linewidth}\r\n" +
                    "    " + entferneUmlautefuerLaTex(Farbname_DieseFarbe) + "\\\\" + Lauflaenge_DieseFarbe_mitEinheit + " (" + Gewicht_DieseFarbe_mitEinheit + ")\r\n" +
                    "  \\end{minipage}\r\n" +
                    "\\end{minipage}\r\n";       
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(name_texfile_wollmengen))
            {
                sw.Write(inhalt_texfile_wollmengen);
            }

            //Maschengroessen aus den Textboxen holen
            float.TryParse(textBox_Maschenbreite.Text, out Breite_Masche);
            float.TryParse(textBox_Maschenhoehe.Text, out Hoehe_Masche);

            GroessenString = GroessenString + Math.Round(Breite_Masche * Originalbild.Width).ToString() + " x " + Math.Round(Hoehe_Masche * Originalbild.Height).ToString() + " cm)\r\n";


            //ermitteln ob das Seitenverhältnis des Motivs sehr breit ist, um in LaTeX zu entscheiden das Rasterbild zu drehen
            string Breitbild = "0";
            if (Rasterbild.Width > Rasterbild.Height)
            {
                Breitbild = "1";
            }


            //Farbnamen des allerersten Pixels ermitteln fuer die Variable in der Texfile
            Zeile_Auswerten(Originalbild.Height-1);
            String Farbnameerstespixel = listView_LineDescription.Items[0].SubItems[7].Text;

            string tex_Titlestring = "";
            if (deu)
            {
                tex_Titlestring = "H\"akelanleitung - Kinderdecke";
            }
            if (eng)
            {
                tex_Titlestring = "Crochet Tutorial - Childrens Blanket";
            }
            inhalt_texfile_Main =
                "\\author{Denise die Wollmaus}\r\n" +
                "\\newcommand{\\motivbreite}{" + Originalbild.Width + "}\r\n" +
                "\\newcommand{\\breitbild}{" + Breitbild + "}\r\n" +
                "\\newcommand{\\erstefarbe}{" + entferneUmlautefuerLaTex(Farbnameerstespixel) + "}\r\n" +
                "\\newcommand{\\deckenbreite}{" + Math.Round(Breite_Masche * Originalbild.Width + 10).ToString() + "}\r\n" +
                "\\newcommand{\\deckenhoehe}{" + Math.Round(Hoehe_Masche * Originalbild.Height + 10).ToString() + "}\r\n" +
                "\\newcommand{\\motivtitel}{" + Bildtitel + "}\r\n" +
                "\\newcommand{\\motivtitelohnesonderzeichen}{" + entferneUmlautefuerDateinamen(Bildtitel) + "}\r\n" +
                "\\title{" + tex_Titlestring + " (\\motivtitel)}\r\n" +
                "\\input{struktur_allgemein" + file_langsuffix + ".tex}\r\n";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(name_texfile_Main))
            {
                sw.Write(inhalt_texfile_Main);
            }
            inhalt_texfile_titelseite =
                "\\begin{center}\r\n" +
                "\\section *{" + tex_Titlestring + " \\\\"+
                "\\Huge\\motivtitel}\r\n" +
                "\\label{ sec: Title}\r\n" +
                "\\end{center}\r\n" +
                "\\begin{center}\r\n" +
                "\\fbox{\\includegraphics[height = 1.00\\textwidth, width = 1.00\\textwidth, keepaspectratio]{../\\motivtitelohnesonderzeichen_Titelbild}}\r\n" +
                "\\end{center}\r\n";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(name_texfile_titelseite))
            {
                sw.Write(inhalt_texfile_titelseite);
            }


            //Beispielzeilen_Anleitung
            int beispielzeilennummer1 = 0;
            int beispielzeilennummer2 = 0;
            int farbenindieserZeile = 0;
            Int32.TryParse(textBox_Beispielreihe1.Text, out beispielzeilennummer1);
            Int32.TryParse(textBox_Beispielreihe2.Text, out beispielzeilennummer2);

            Zeile_Auswerten(Originalbild.Height - beispielzeilennummer1);
            farbenindieserZeile = (listView_LineDescription.Items.Count - 1);
            String Beispielzeile1 = "";
            if (deu)
            {
                Beispielzeile1 = "\\textbf{Beispiele: }Zur Verdeutlichung wie gez\"ahlt wird, hier eine ausf\"uhrliche Beschreibung für Zeile " + beispielzeilennummer1 + " von unten (erste Zeile mit Farbwechseln). Wir fangen auf der rechten Seite des Motivs an zu z\"ahlen. ";
            }
            if (eng)
            {
                Beispielzeile1 = "\\textbf{Examples: }For clarification on how to count, this is a detailed description of line " + beispielzeilennummer1 + " from the bottom (first line with color changes). We start counting from the right side of the pattern. ";
            }

            foreach (ListViewItem Item in listView_LineDescription.Items)
            {
                if (Item.Index == 0)
                {
                    //Anleitungssatz fuer die erste Farbe formulieren
                    if (deu)
                    {
                        Beispielzeile1 = Beispielzeile1 + "Zuerst werden " + Item.Text + " Maschen in " + Item.SubItems[7].Text + " gearbeitet. ";
                    }
                    if (eng)
                    {
                        Beispielzeile1 = Beispielzeile1 + "First we work " + Item.Text + " stitches in " + Item.SubItems[7].Text + ". ";
                    }
                }
                else if (Item.Index == listView_LineDescription.Items.Count - 1)
                {
                    //Anleitungssatz fuer die letzte Farbe formulieren
                    if (deu)
                    {
                        Beispielzeile1 = Beispielzeile1 + "Zum Schluss wird die Zeile noch mit " + Item.Text + " Maschen in " + Item.SubItems[7].Text + " beendet. ";
                    }
                    if (eng)
                    {
                        Beispielzeile1 = Beispielzeile1 + "Finally we end the row with " + Item.Text + " stitches in " + Item.SubItems[7].Text + ". ";
                    }
                }
                else
                {
                    //Anleitungssatz fuer die Farben in der Mitte formulieren
                    if (deu)
                    {
                        Beispielzeile1 = Beispielzeile1 + "Dann " + Item.Text + " in " + Item.SubItems[7].Text + ". ";
                    }
                    if (eng)
                    {
                        Beispielzeile1 = Beispielzeile1 + "Then " + Item.Text + " in " + Item.SubItems[7].Text + ". ";
                    }
                }
            }

            Zeile_Auswerten(Originalbild.Height - beispielzeilennummer2);
            farbenindieserZeile = (listView_LineDescription.Items.Count - 1);
            String Beispielzeile2 = "";
            if (deu)
            {
                Beispielzeile2 = "Als zweites Beispiel noch Zeile " + beispielzeilennummer2 +". Wir beginnen wieder auf der rechten Seite. ";
            }
            if (eng)
            {
                Beispielzeile2 = "For a second example we now look at line " + beispielzeilennummer2 + ". Again we start from the right side. ";
            }

            foreach (ListViewItem Item in listView_LineDescription.Items)
            {
                if (Item.Index == 0)
                {
                    //Anleitungssatz fuer die erste Farbe formulieren
                    if (deu)
                    {
                        Beispielzeile2 = Beispielzeile2 + "Zuerst werden " + Item.Text + " Maschen in " + Item.SubItems[7].Text + " gearbeitet. ";
                    }
                    if (eng)
                    {
                        Beispielzeile2 = Beispielzeile2 + "First we work " + Item.Text + " stitches in " + Item.SubItems[7].Text + ". ";
                    }
                }
                else if (Item.Index == listView_LineDescription.Items.Count - 1)
                {
                    //Anleitungssatz fuer die letzte Farbe formulieren
                    if (deu)
                    {
                        Beispielzeile2 = Beispielzeile2 + "Zum Schluss wird die Zeile noch mit " + Item.Text + " Maschen in " + Item.SubItems[7].Text + " beendet. ";
                    }
                    if (eng)
                    {
                        Beispielzeile2 = Beispielzeile2 + "Finally we end the row with " + Item.Text + " stitches in " + Item.SubItems[7].Text + ". ";
                    }
                }
                else
                {
                    //Anleitungssatz fuer die Farben in der Mitte formulieren
                    if (deu)
                    {
                        Beispielzeile2 = Beispielzeile2 + "Dann " + Item.Text + " in " + Item.SubItems[7].Text + ". ";
                    }
                    if (eng)
                    {
                        Beispielzeile2 = Beispielzeile2 + "Then " + Item.Text + " in " + Item.SubItems[7].Text + ". ";
                    }
                }
            }

            inhalt_texfile_beispielreihen = entferneUmlautefuerLaTex(Beispielzeile1) + "\r\n\r\n" + entferneUmlautefuerLaTex(Beispielzeile2);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(name_texfile_beispielreihen))
            {
                sw.Write(inhalt_texfile_beispielreihen);
            }

            inhalt_Infofile = IntroString + GroessenString + "\r\n" + MaschenzahlenString + "\r\n" + LauflaengenString + "\r\n" + Beispielzeile1 + "\r\n\r\n" + Beispielzeile2;

            //Infodatei erzeugen
            string filename = String.Format(Bildtitel + "_Dateien/" + Bildtitel + "_Info.txt");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            {
                sw.Write(inhalt_Infofile);
            }

            //pdf-generierung anstossen
            //System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex.exe --output-directory=../ " + name_texfile_Main); //funktioniert so nicht...

        }

        private void createImagefiles()
        {
            if (checkBox_AutoRasterfarbe.Checked)
            {
                FindeIdealeRasterfarbe();
            }

            Color Rasterfarbe = HextoColor(textBox_Rasterbild_Linienfarbe.Text);
            Brush Rasterbrush1 = new SolidBrush(Rasterfarbe);
            progressBar1.Minimum = 1;
            progressBar1.Value = 1;
            progressBar1.Maximum = Originalbild.Height;
            int Pixelgroesse = (int)numericUpDown_Rasterbild_Pixelgroesse.Value;
            int Linie_1 = (int)numericUpDown_Rasterbild_Liniendicke_1.Value;
            int Linie_5 = (int)numericUpDown_Rasterbild_Liniendicke_5.Value;
            int Linie_10 = (int)numericUpDown_Rasterbild_Liniendicke_10.Value;

            //Resultierende Bildgröße berechnen
            int newWidth = Originalbild.Width * Pixelgroesse;                               //Breite ohne Trennlinien
            newWidth = newWidth + ((Originalbild.Width - 1) * Linie_1);                     //+ Breite der 1-er Linien
            newWidth = newWidth + ((Originalbild.Width - 1) / 5) * (Linie_5 - Linie_1);     //+Breite der 5-er Linien  minus breite der wegfallenden 1er linien
            newWidth = newWidth + ((Originalbild.Width - 1) / 10) * (Linie_10 - Linie_5);   //+Breite der 10-er Linien  minus breite der wegfallenden 5er linien
            newWidth = newWidth + (Linie_10 * 2);                                           //+Platz für eine Bildumrandung in Dicke der 10er Linien

            int newHeight = Originalbild.Height * Pixelgroesse;                             //Hoehe ohne Trennlinien
            newHeight = newHeight + ((Originalbild.Height - 1) * Linie_1);                     //+ Breite der 1-er Linien
            newHeight = newHeight + ((Originalbild.Height - 1) / 5) * (Linie_5 - Linie_1);     //+ Breite der 5-er Linien  minus breite der wegfallenden 1er linien
            newHeight = newHeight + ((Originalbild.Height - 1) / 10) * (Linie_10 - Linie_5);   //+ Breite der 10-er Linien  minus breite der wegfallenden 5er linien
            newHeight = newHeight + (Linie_10 * 2);                                            //+Platz für Bildumrandung in Dicke der 10er Linien



            //Image anlegen
            Rasterbild = new Bitmap(newWidth, newHeight);

            //Hintergrund mit Rasterfarbe füllen... spart das spätere zeichnen des Rasters
            using (Graphics gr = Graphics.FromImage(Rasterbild))
            {
                Rectangle r = new Rectangle(0, 0, newWidth, newHeight); //TODO korrigieren, stimmt vermutlich nicht
                gr.FillRectangle(Rasterbrush1, r);
            }

            int cursorY = newHeight - Linie_10;


            for (int yy = 1; yy <= Originalbild.Height; yy++)
            {
                int cursorX = newWidth - Linie_10;
                Application.DoEvents(); //damit während der heftigen schleife die Progressbar vom thread aktualisiert werden kann
                progressBar1.Value = yy;
                for (int xx = 1; xx <= Originalbild.Width; xx++)
                {
                    //Pixelfarbe des Originalbilds ermitteln
                    Color Pixelcolor = Originalbild.GetPixel(Originalbild.Width - xx, Originalbild.Height - yy);

                    //Pixel zeichnen
                    Brush Pixelbrush = new SolidBrush(Pixelcolor);
                    using (Graphics gr = Graphics.FromImage(Rasterbild))
                    {
                        Rectangle r = new Rectangle(cursorX - Pixelgroesse, cursorY - Pixelgroesse, Pixelgroesse, Pixelgroesse); //TODO korrigieren, stimmt vermutlich nicht
                        gr.FillRectangle(Pixelbrush, r);
                    }
                    cursorX = cursorX - Pixelgroesse;

                    //prüfen ob 1te, 5te oder 10te Spalte und cursor verschieben
                    if (xx % 10 == 0)
                    {//10te Spalte
                        cursorX = cursorX - Linie_10;
                    }
                    else if (xx % 5 == 0)
                    {//5te Spalte
                        cursorX = cursorX - Linie_5;
                    }
                    else
                    {//1te Spalte
                        cursorX = cursorX - Linie_1;
                    }
                }
                cursorY = cursorY - Pixelgroesse;

                //IF AUSKOMMENTIERT, AUSWAHL-CHECKBOX ENTFERNT... IST JETZT STANDARD
                //Falls horizontales Raster eingestellt ist, die entsprechende Linie freilassen...
                //if (checkBox_Rasterbild_horizontal_auch.Checked)
                //{
                    if (yy % 10 == 0)
                    {//10te Zeile
                        cursorY = cursorY - Linie_10;
                    }
                    else if (yy % 5 == 0)
                    {//5te Zeile
                        cursorY = cursorY - Linie_5;
                    }
                    else
                    {//1te Zeile
                        cursorY = cursorY - Linie_1;
                    }
                //}
            }

            //Originalbild als bmp ablegen
            string filename = Bildtitel + "_Dateien/" + entferneUmlautefuerDateinamen(Bildtitel) + "_Originalbild.bmp";
            Originalbild.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);

            //Originalbild ein zweites mal als vorlaufiges Titelbild png abspeichern
            filename = Bildtitel + "_Dateien/" + entferneUmlautefuerDateinamen(Bildtitel) + "_Titelbild.png";
            Originalbild.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            //generiertes Rasterbild speichern

            //erste Version als bmp für maximale Qualitaet
            //filename = Bildtitel + "_Dateien/" + Bildtitel + "_Rasterbild.bmp";
            //Rasterbild.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);

            //zweite Version als png für weniger Speicherbedarf
            filename = Bildtitel + "_Dateien/" + entferneUmlautefuerDateinamen(Bildtitel) + "_Rasterbild.png";
            Rasterbild.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            //Rasterbild mit Viewer öffnen
            //Process.Start(filename);
        }

        private void createPDF()
        {
            bool eng = radioButton_englisch.Checked;
            bool deu = radioButton_deutsch.Checked;
            string filename = "";
            // Create a temporary file

            //string filename = String.Format(  Bildtitel + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +   ".pdf");
            if (deu)
            {
                filename = String.Format(Bildtitel + "_Dateien/" + "Vorlage_" + entferneUmlautefuerDateinamen(Bildtitel)+ ".pdf");
            }
            if (eng)
            {
                filename = String.Format(Bildtitel + "_Dateien/" + "Pattern_" + entferneUmlautefuerDateinamen(Bildtitel) + ".pdf");
            }
            myPDF = new PdfDocument();
            if (deu)
            {
                myPDF.Info.Title = "Vorlage - " + Bildtitel;
                myPDF.Info.Subject = "Vorlage";
            }
            if (eng)
            {
                myPDF.Info.Title = "Pattern - " + Bildtitel;
                myPDF.Info.Subject = "Pattern";
            }
                myPDF.Info.Keywords = "XGraphics";
                myPDF.Info.Author = "Denise die Wollmaus";


            ////////// Titelseite erstellen ////////////
            PdfPage page = myPDF.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            if (deu)
            {
                DrawTitle(page, gfx, "Vorlage - " + Bildtitel);
            }
            if (eng)
            {
                DrawTitle(page, gfx, "Pattern - " + Bildtitel);
            }


            XRect rect_untertitel = new XRect(new XPoint(), gfx.PageSize);
            rect_untertitel.Inflate(-10, -40);
            XFont font_untertitel = new XFont("Times", 12, XFontStyle.Bold);

            if (deu)
            {
                gfx.DrawString("(" + Originalbild.Height + " Reihen und " + Originalbild.Width + " Spalten)", font_untertitel, XBrushes.Black, rect_untertitel, XStringFormats.TopCenter);
            }
            if (eng)
            {
                gfx.DrawString("(" + Originalbild.Height + " rows and " + Originalbild.Width + " columns)", font_untertitel, XBrushes.Black, rect_untertitel, XStringFormats.TopCenter);
            }

            XPen pen_Line = new XPen(Color.Black, 1.5);
            gfx.DrawLine(pen_Line, 10, 58, page.Width - 10, 58);

            //DrawPagenumber(page, gfx);


            XRect rect = new XRect(new XPoint(), gfx.PageSize);

            double Bild_maxwidth = 575;
            double Bild_maxheight = 742;
            double Bild_minX = 10;
            double Bild_minY = 60;

            double Bild_targetwidth = 0;
            double Bild_targetheight = 0;
            double Bild_targetx = 0;
            double Bild_targety = 0;


            double Bild_PlatzhalterRatio = Bild_maxwidth / Bild_maxheight;
            double Bildratio = (double)Zoombild.Width / (double)Zoombild.Height;

            if (Bild_PlatzhalterRatio > Bildratio)
            {//Box ist zu Breit, Bild stösst in der Höhe zuerst an
                Bild_targetheight = Bild_maxheight;
                Bild_targetwidth = Bildratio * Bild_targetheight;
            }
            else
            {//Box ist zu hoch, Bild stösst in der Höhe zuerst an
                Bild_targetwidth = Bild_maxwidth;
                Bild_targetheight = Bild_targetwidth / Bildratio;
            }
            Bild_targetx = ((Bild_maxwidth - Bild_targetwidth) / 2) + Bild_minX;
            Bild_targety = ((Bild_maxheight - Bild_targetheight) / 2) + Bild_minY;
            DrawImageScaled(gfx, Zoombild, (int)Bild_targetx, (int)Bild_targety, (int)Bild_targetwidth, (int)Bild_targetheight);


            XPen pen = new XPen(XColors.Black, 2);



            ////////////////////////// Design der Beschreibungsseiten ////////////////////////////

            Bild_maxwidth = 575;
            Bild_maxheight = 440;
            Bild_PlatzhalterRatio = Bild_maxwidth / Bild_maxheight;



            int SeitenZahl = 1;
            double Lines_Abstand_linksrechts = 10;
            double Lines_Abstand_oben = 520;
            double Lines_Abstand_unten = 10;

            double Line_OffsetY = 50;
            double Line_OffsetX = 20;
            double Colorbox_breite = 40;
            double Colorbox_Rahmendicke = 2;
            XColor Colorbox_Rahmenfarbe = XColors.Black;

            XColor Trennlinienfarbe = XColors.Gray;
            double Trennliniendicke = 1.5;

            double Text_AbstandZuColorbox = 3;
            double Text_OffsetY_Zeilen = 24;
            int Text_Schriftgrad_Pixelcount = 26;
            int Text_Schriftgrad_Farbname = 18;
            string Text_Schriftname = "Calibri Bold";
            XColor Text_Farbe = XColors.Black;

            XFont font_Pixelcount = new XFont(Text_Schriftname, Text_Schriftgrad_Pixelcount, XFontStyle.Regular);
            XFont font_Farbname = new XFont(Text_Schriftname, Text_Schriftgrad_Farbname, XFontStyle.Regular);
            XPen pen_Colorbox_Rahmen = new XPen(Colorbox_Rahmenfarbe, Colorbox_Rahmendicke);
            XPen pen_zwischenlinien = new XPen(Trennlinienfarbe, Trennliniendicke);



            //////////////////////// Beschreibungsseite anlegen ///////////////////////////

            if (Bild_PlatzhalterRatio > Bildratio)
            {//Box ist zu Breit, Bild stösst in der Höhe zuerst an
                Bild_targetheight = Bild_maxheight;
                Bild_targetwidth = Bildratio * Bild_targetheight;
            }
            else
            {//Box ist zu hoch, Bild stösst in der Höhe zuerst an
                Bild_targetwidth = Bild_maxwidth;
                Bild_targetheight = Bild_targetwidth / Bildratio;
            }
            Bild_targetx = ((Bild_maxwidth - Bild_targetwidth) / 2) + Bild_minX;
            Bild_targety = ((Bild_maxheight - Bild_targetheight) / 2) + Bild_minY;

            //XForms Objekt für ein wiederverwendbares Bild
            XForm xform_Bild = new XForm(myPDF, Bild_targetwidth, Bild_targetheight);
            XGraphics formGfx = XGraphics.FromForm(xform_Bild);
            formGfx.DrawImage(Zoombild, 0, 0, (int)Bild_targetwidth, (int)Bild_targetheight);





            progressBar1.Minimum = 1;
            progressBar1.Value = 1;
            progressBar1.Maximum = Originalbild.Height;
            progressBar1.Visible = true;
            for (int i = 1; i <= Originalbild.Height; i++)
            {
                Application.DoEvents(); //damit während der heftigen schleife die Progressbar vom thread aktualisiert werden kann
                progressBar1.Value = i;
                numericUpDown_Zeile.Value = i;
                selectedLine = Originalbild.Height - (int)numericUpDown_Zeile.Value;
                Zeile_Auswerten(selectedLine);

                //neue Seite anlegen
                page = myPDF.AddPage();
                gfx = XGraphics.FromPdfPage(page);

                //Bild auf Beschreibungsseite einfügen...


                //DrawImageScaled(gfx, Displaybild, (int)Bild_targetx, (int)Bild_targety, (int)Bild_targetwidth, (int)Bild_targetheight);

                //yform - Variante
                gfx.DrawImage(xform_Bild, (int)Bild_targetx, (int)Bild_targety, (int)Bild_targetwidth, (int)Bild_targetheight);

                //Zeile markieren...
                double ZoomFactorY = Bild_targetheight / Originalbild.Height;
                Rectangle markierung = new Rectangle((int)Bild_targetx, (int)(Math.Round((Originalbild.Height - i) * ZoomFactorY) + Bild_targety), (int)Bild_targetwidth - 1, (int)(Math.Round(ZoomFactorY)));
                XPen markierungspen1 = new XPen(Color.White, 1);
                XPen markierungspen2 = new XPen(Color.Black, 1);
                markierungspen2.DashStyle = XDashStyle.Custom;
                markierungspen2.DashPattern = new double[] { 2, 2 };

                gfx.DrawRectangle(markierungspen1, markierung);
                gfx.DrawRectangle(markierungspen2, markierung);



                double xpos = Lines_Abstand_linksrechts;
                double ypos = Lines_Abstand_oben;

                gfx.DrawLine(pen_zwischenlinien, 10, ypos - 5, page.Width - 10, ypos - 5);

                foreach (ListViewItem item in listView_LineDescription.Items)
                {
                    Color myFillColor = System.Drawing.ColorTranslator.FromHtml(item.SubItems[1].Text);
                    string myPixelcount = item.Text;
                    string myColorname = item.SubItems[7].Text;

                    double noetigeBreite = gfx.MeasureString(myPixelcount, font_Pixelcount).Width;
                    if (gfx.MeasureString(myColorname, font_Farbname).Width > noetigeBreite)
                    {
                        noetigeBreite = gfx.MeasureString(myColorname.ToString(), font_Farbname).Width;
                    }
                    noetigeBreite = noetigeBreite + Colorbox_breite + Text_AbstandZuColorbox;

                    //falls die nächste Farbe nicht mehr in die Zeile passt, eine neue anfangen...
                    if (xpos + noetigeBreite + Lines_Abstand_linksrechts > page.Width)
                    {

                        xpos = 10;
                        ypos = ypos + Line_OffsetY;

                        //falls die neue Zeile nicht mehr auf die Seite passt, eine neue anfangen...
                        if (ypos + Line_OffsetY + Lines_Abstand_unten > page.Height)
                        {
                            if (deu)
                            {
                                DrawTitle(page, gfx, "Reihe " + i + " -  Seite " + SeitenZahl);
                            }
                            if (eng)
                            {
                                DrawTitle(page, gfx, "Row " + i + " -  Page " + SeitenZahl);
                            }
                            page = myPDF.AddPage();
                            gfx = XGraphics.FromPdfPage(page);



                            SeitenZahl++;
                            xpos = Lines_Abstand_linksrechts;
                            ypos = Lines_Abstand_oben;

                            //DrawImageScaled(gfx, Displaybild, (int)Bild_targetx, (int)Bild_targety, (int)Bild_targetwidth, (int)Bild_targetheight);

                            //xform-Variante;
                            gfx.DrawImage(xform_Bild, (int)Bild_targetx, (int)Bild_targety, (int)Bild_targetwidth, (int)Bild_targetheight);
                            gfx.DrawRectangle(markierungspen1, markierung);
                            gfx.DrawRectangle(markierungspen2, markierung);

                        }
                        else
                        {
                            gfx.DrawLine(pen_zwischenlinien, 10, ypos - 5, page.Width - 10, ypos - 5);
                        }

                    }


                    XRect myrect = new XRect(xpos + Colorbox_breite + Text_AbstandZuColorbox, ypos, page.Width - xpos, page.Height - ypos);

                    gfx.DrawRectangle(pen_Colorbox_Rahmen, new XSolidBrush(XColor.FromArgb(myFillColor)), xpos, ypos, Colorbox_breite, Colorbox_breite);
                    gfx.DrawString(myPixelcount, font_Pixelcount, XBrushes.Black, myrect, XStringFormats.TopLeft);
                    myrect.Offset(0, Text_OffsetY_Zeilen);
                    gfx.DrawString(myColorname, font_Farbname, XBrushes.Black, myrect, XStringFormats.TopLeft);

                    xpos = xpos + noetigeBreite + Line_OffsetX;

                }
                if (SeitenZahl > 1)
                {
                    if (deu)
                    {
                        DrawTitle(page, gfx, "Reihe " + i + " -  Seite " + SeitenZahl);
                    }
                    if (eng)
                    {
                        DrawTitle(page, gfx, "Row " + i + " -  Page " + SeitenZahl);
                    }
                }
                else
                {
                    if (deu)
                    {
                        DrawTitle(page, gfx, "Reihe " + i);
                    }
                    if (eng)
                    {
                        DrawTitle(page, gfx, "Row " + i);
                    }
                }
                SeitenZahl = 1;

            }



            // Save the pdf...
            myPDF.Save(filename);
            // ...and start a viewer
            //Process.Start(filename);
        }

        public void DrawTitle(PdfPage page, XGraphics gfx, string title)
        {
            XRect rect = new XRect(new XPoint(), gfx.PageSize);
            rect.Inflate(-10, -15);
            XFont font = new XFont("Times", 20, XFontStyle.Bold);
            gfx.DrawString(title, font, XBrushes.Black, rect, XStringFormats.TopCenter);

            rect.Offset(0, 5);
            font = new XFont("Verdana", 8, XFontStyle.Italic);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Far;
            gfx.DrawString("created by denisediewollmaus@gmail.com", font, XBrushes.Navy, rect, format);

            myPDF.Outlines.Add(title, page, true);
        }

        private void DrawPagenumber(PdfPage page, XGraphics gfx)
        {
            XRect rect = new XRect(new XPoint(), gfx.PageSize);
            rect.Inflate(-10, -15);
            rect.Offset(0, 5);
            XFont font = new XFont("Verdana Italic", 10);
            XStringFormat format = new XStringFormat();
            format.LineAlignment = XLineAlignment.Far;
            format.Alignment = XStringAlignment.Center;
            gfx.DrawString(myPDF.PageCount.ToString(), font, XBrushes.Gray, rect, format);
        }

        void DrawImageScaled(XGraphics gfx, Image Image, int X, int Y, int Width, int Height)
        {
            XImage image = XImage.FromGdiPlusImage(Image);
            gfx.DrawImage(image, X, Y, Width, Height);
        }


        //Config-Funktionen
        private void savecolornamestoConfig()
        {
            bool deu = radioButton_deutsch.Checked;
            bool eng = radioButton_englisch.Checked;

            //Die Farbnamen aus der Palette zur Dictionary hinzufügen
            foreach (ListViewItem p in listView_Palette.Items)
            {
                String Farbcode = p.SubItems[1].Text;
                String Farbname = p.Text;

                if (deu)
                {
                    if (DictColorNames_DE.ContainsKey(Farbcode))
                    {
                        //falls bereits vorhanden wird mit dem neuen Namen ueberschrieben
                        DictColorNames_DE[Farbcode] = Farbname;
                    }
                    else
                    {
                        //falls noch nicht vorhanden wird das keyvaluepaar angelegt
                        DictColorNames_DE.Add(Farbcode, Farbname);
                    }
                }
                if (eng)
                {
                    if (DictColorNames_EN.ContainsKey(Farbcode))
                    {
                        //falls bereits vorhanden wird mit dem neuen Namen ueberschrieben
                        DictColorNames_EN[Farbcode] = Farbname;
                    }
                    else
                    {
                        //falls noch nicht vorhanden wird das keyvaluepaar angelegt
                        DictColorNames_EN.Add(Farbcode, Farbname);
                    }
                }
            }

            //Aus der Dictionary die ColorNameList für die Config bilden
            String_ColorNameList_DE = "";

            foreach (KeyValuePair<string, string> entry in DictColorNames_DE)
            {
                //Farbcode und Namen zur ColorNameList hinzufuegen
                String_ColorNameList_DE = String_ColorNameList_DE + entry.Key + "," + entry.Value + ";";
            }

            //letztes Semikolon entfernen
            if (String_ColorNameList_DE.EndsWith(";"))
            {
                String_ColorNameList_DE = String_ColorNameList_DE.Remove(String_ColorNameList_DE.Length - 1);
            }


            //Aus der Dictionary die ColorNameList für die Config bilden
            String_ColorNameList_EN = "";

            foreach (KeyValuePair<string, string> entry in DictColorNames_EN)
            {
                //Farbcode und Namen zur ColorNameList hinzufuegen
                String_ColorNameList_EN = String_ColorNameList_EN + entry.Key + "," + entry.Value + ";";
            }

            //letztes Semikolon entfernen
            if (String_ColorNameList_EN.EndsWith(";"))
            {
                String_ColorNameList_EN = String_ColorNameList_EN.Remove(String_ColorNameList_EN.Length - 1);
            }

            SetSetting("ColorNameList_DE", String_ColorNameList_DE);
            SetSetting("ColorNameList_EN", String_ColorNameList_EN);
        }

        private static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[key] == null)
            {
                configuration.AppSettings.Settings.Add(key, value);
            }
            else
            {
                configuration.AppSettings.Settings[key].Value = value;
            }
         
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("AppSettings");
        }

    }
}
