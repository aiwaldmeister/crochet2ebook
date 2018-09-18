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
        private Bitmap Originalbild;
        private Bitmap Zoombild;
        private Bitmap Displaybild;
        private Bitmap Rasterbild;
        private bool DisplayRatioCorrection = true;
        private double RatioCorrFactor = 0.6;
        private int additionalZoomfactor = 1;
        private bool imageisloaded = false;
        private int selectedLine = 1;
        private PdfDocument myPDF;
        private Dictionary<string, string> DictColorNames = new Dictionary<string, string>();
        private string String_ColorNameList = "";


        private void button_Palette_generatefromImage_Click(object sender, EventArgs e)
        {
            generatePalettefromImage();
        }

        public static String ColortoHex(Color Col)
        {
            return ColorTranslator.ToHtml(Col);
        }

        public static Color HextoColor(String colorCode)
        {
            return System.Drawing.ColorTranslator.FromHtml(colorCode);
        }

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

        private void addNewColor(String colorCode)
        {
            
            String colorName = colorCode;

            //Namen für die Farbe finden
            if (DictColorNames.Keys.Contains(colorCode))
            {
                colorName = DictColorNames[colorCode];
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

        private void button_LoadImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog_Picture.ShowDialog() == DialogResult.OK)
            {

                Dateiname = openFileDialog_Picture.FileName;
                ladeBild();

                textBox_Titel.Text = System.IO.Path.GetFileNameWithoutExtension(Dateiname);
                Bildtitel = textBox_Titel.Text;
                this.Text = Bildtitel + " - Pixelcounter 2.0";

                numericUpDown1.Value = 1;
                selectedLine = Originalbild.Height - (int)numericUpDown1.Value;
                Zeile_Auswerten(selectedLine);
                
            }
        }

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
                numericUpDown1.Enabled = true;
                button_Zoom.Enabled = true;
                button_create_stuff.Enabled = true;
                
                numericUpDown1.Maximum = Originalbild.Height;
                
            
        }

        private void selectLine(int v)
        {
            selectedLine = v;
        }

        private void generateZoomedImage()
        {
            if (DisplayRatioCorrection)
            {
                RatioCorrFactor = 0.6;
            }
            else
            {
                RatioCorrFactor = 1;
            }
            int newWidth, newHeight;

            int maxWidth = splitContainer1.Panel2.Width-2;
            int maxHeight = splitContainer1.Panel2.Height-2;

            if (((double)Originalbild.Width * RatioCorrFactor) / ((double)Originalbild.Height) > (double)maxWidth / (double)maxHeight)
            {//Bild ist (mit korrekturfaktor) breiter als hoch... breite auf breite der Picturebox setzen, höhe verhältnis
                newWidth = maxWidth;
                newHeight = (int)((((double)maxWidth / (double)Originalbild.Width) * (double)Originalbild.Height) / RatioCorrFactor);
            }
            else
            {//Bild ist (mit korrekturfaktor) hoeher als breit... hoehe auf hoehe der Picturebox setzen, breite im verhältnis
                newHeight = (int)((double)maxHeight);
                newWidth = (int)((((double)maxHeight / (double)Originalbild.Height) * (double)Originalbild.Width) * RatioCorrFactor);
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

        private void refreshDisplay()
        {
              
              pictureBox_Display.Image = Displaybild;
//            pictureBox_Display.Refresh();
//            pictureBox_Display.Update();
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

                int.TryParse(selection.SubItems[2].Text, out startpixel);
                int.TryParse(selection.SubItems[3].Text, out pixelanzahl);

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

        private float getZoomFactorX()
        {
            return (float)Zoombild.Width / (float)Originalbild.Width;
        }

        private float getZoomFactorY()
        {
            return (float)Zoombild.Height / (float)Originalbild.Height;
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

        private void button1_Click(object sender, EventArgs e)
        {
           textBox1.Text =  Zeile_Auswerten(Originalbild.Height -1);
            Zeilemarkieren(Originalbild.Height - 1);
        }

        private void dieseFarbeUmbenennenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView_Palette.FocusedItem.BeginEdit();
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
        
        private void button_Zoom_Click(object sender, EventArgs e)
        {
            toggleZoom();
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
        
        private void listView_LineDescription_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Zeilemarkieren(selectedLine);

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            selectedLine = Originalbild.Height - (int)numericUpDown1.Value;
            Zeile_Auswerten(selectedLine);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView_LineDescription.Clear();

            string iniDateiname = GetSetting("Image");
            string iniLine = GetSetting("Line");
            string iniTitle = GetSetting("Title");
            string iniDisplayRatioCorrection = GetSetting("DisplayRatioCorrection");
            string iniColorNameList = GetSetting("ColorNameList");
                        
            int iniLineint = 1;
            int.TryParse(iniLine, out iniLineint);
            
            splitContainer4.Panel2Collapsed = true;

            //initialisiere die Dictionary mit den Farbnamen aus dem String iniColorNameList
            string[] iniFarbstrings = iniColorNameList.Split(';');
            foreach (String Farbstring in iniFarbstrings)
            {
                if (!Farbstring.Equals(""))
                {
                    DictColorNames.Add(Farbstring.Substring(0,7),Farbstring.Substring(8));
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
                numericUpDown1.Value = iniLineint;
                selectedLine = Originalbild.Height - (int)numericUpDown1.Value;
                Zeile_Auswerten(selectedLine);
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            //Die Farbnamen aus der Palette zur Dictionary hinzufügen
            foreach (ListViewItem p in listView_Palette.Items)
            {
                String Farbcode = p.SubItems[1].Text;
                String Farbname = p.Text;

                if (DictColorNames.ContainsKey(Farbcode))
                {
                    //falls bereits vorhanden wird mit dem neuen Namen ueberschrieben
                    DictColorNames[Farbcode] = Farbname;
                }
                else
                {
                    //falls noch nicht vorhanden wird das keyvaluepaar angelegt
                    DictColorNames.Add(Farbcode, Farbname);
                }

            }

            //Aus der Dictionary die ColorNameList für die Config bilden
            String_ColorNameList = "";

            foreach (KeyValuePair<string, string> entry in DictColorNames)
            {
                //Farbcode und Namen zur ColorNameList hinzufuegen
                String_ColorNameList = String_ColorNameList + entry.Key + "," + entry.Value + ";";
            }

            //letztes Semikolon entfernen
            if(String_ColorNameList.EndsWith(";"))
            {
                String_ColorNameList = String_ColorNameList.Remove(String_ColorNameList.Length - 1);
            }
            


            //Status, Settings und Farbnamen in die Config schreiben
            SetSetting("Image", Dateiname);
            SetSetting("Title", Bildtitel);
            SetSetting("Line",numericUpDown1.Value.ToString());
            if (DisplayRatioCorrection)
            {
                SetSetting("DisplayRatioCorrection", "1");
            }else{
                SetSetting("DisplayRatioCorrection", "0");
            }
            SetSetting("ColorNameList", String_ColorNameList);
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



            System.IO.Directory.CreateDirectory(Bildtitel + "_Dateien");

            //PDF generieren
            if (checkBox_pdf.Checked)
            {
                createPDF();
            }

            //Rasterbild generieren
            if (checkBox_Rasterbild.Checked)
            {
                createRasterbild();
            }


            //Maschen und Farbwechsel ermitteln
            countMaschenUndFarbwechsel(Originalbild);

            //Infodatei generieren
            if (checkBox_InfoDatei.Checked)
            {
                createInfofile();
            }
            
            //Verzeichnis mit Ergebnissen öffnen
            Process.Start("explorer.exe", Bildtitel + "_Dateien");
            
            //alle Generierungen beendet... alles zurück auf Anfang...
            progressBar1.Visible = false;

            numericUpDown1.Value = 1;
            selectedLine = Originalbild.Height - (int)numericUpDown1.Value;
            Zeile_Auswerten(selectedLine);

        }

        private void countMaschenUndFarbwechsel(Bitmap Bild)
        {
            Color pixelfarbe_current;
            Color pixelfarbe_last;

            const int Maschenanzahl = 1;
            const int Farbanfang = 2;
            const int Farbende = 3;

            //Die Zähler für jede Farbe leeren
            foreach (ListViewItem item in listView_Palette.Items)
            {
                item.SubItems[1].Text = 0.ToString();
                item.SubItems[2].Text = 0.ToString();
                item.SubItems[3].Text = 0.ToString();
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
                        increaseSubitemCounter(pixelfarbe_last, Farbende);
                    }
                    pixelfarbe_last = pixelfarbe_current;
                }
            }

            increaseSubitemCounter(pixelfarbe_last, Farbende);
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

        private void createInfofile()
        {
            float Lauflaenge_Masche = 0;
            float Lauflaenge_Wechsel = 0;
            float Breite_Masche = 0;
            float Hoehe_Masche = 0;
            string fileinhalt = "";
            string LauflaengenString = "Lauflängen ca.:\r\n----------------\r\n";
            string MaschenzahlenString = "Maschenanzahl:\r\n--------------\r\n";
            string GroessenString = "(" + Originalbild.Width + " x " + Originalbild.Height + " Maschen / ca. ";
            string IntroString = "Häkeldecke '" + Bildtitel + "'\r\n";
            
            
            
            //Lauflaengen_Werte aus den Textboxen holen
            float.TryParse(textBox_Lauflaenge_Masche.Text, out Lauflaenge_Masche);
            float.TryParse(textBox_Lauflaenge_Wechsel.Text, out Lauflaenge_Wechsel);


            foreach (ListViewItem item in listView_Palette.Items)
            {
                MaschenzahlenString = MaschenzahlenString + item.Text + ": " + item.SubItems[1].Text + "\r\n";

                float Lauflaenge_DieseFarbe = 0;
                int Maschenzahl_DieseFarbe = 0;
                int Wechsel_DieseFarbe = 0;

                Int32.TryParse(item.SubItems[1].Text, out Maschenzahl_DieseFarbe);
                Int32.TryParse(item.SubItems[2].Text, out Wechsel_DieseFarbe);

                Lauflaenge_DieseFarbe = (Lauflaenge_Masche * Maschenzahl_DieseFarbe) + (Lauflaenge_Wechsel * Wechsel_DieseFarbe);

                if (Lauflaenge_DieseFarbe > 100)
                {
                    //als Meter ausgeben
                    LauflaengenString = LauflaengenString + item.Text + ": " + Math.Ceiling(Lauflaenge_DieseFarbe*0.01).ToString() + " m\r\n";
                }
                else
                {
                    //als Zentimeter ausgeben
                    LauflaengenString = LauflaengenString + item.Text + ": " + Math.Ceiling(Lauflaenge_DieseFarbe).ToString() + " cm\r\n";
                }


            }

            //Maschengroessen aus den Textboxen holen
            float.TryParse(textBox_Maschenbreite.Text, out Breite_Masche);
            float.TryParse(textBox_Maschenhoehe.Text, out Hoehe_Masche);

            GroessenString = GroessenString + Math.Round(Breite_Masche * Originalbild.Width).ToString() + " x " + Math.Round(Hoehe_Masche * Originalbild.Height).ToString() + " cm)\r\n";



            fileinhalt = IntroString + GroessenString + "\r\n" + MaschenzahlenString + "\r\n" + LauflaengenString;


            //Infodatei erzeugen
            string filename = String.Format(Bildtitel + "_Dateien/" + Bildtitel + "_Info.txt");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
            {
                sw.Write(fileinhalt);
            }


        }

        private void createRasterbild()
        {
            Color Linecolor1 = HextoColor(textBox_Rasterbild_Linienfarbe1.Text);
            Brush Rasterbrush1 = new SolidBrush(Linecolor1);
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
            if (checkBox_Rasterbild_horizontal_auch.Checked)
            {
                newHeight = newHeight + ((Originalbild.Height - 1) * Linie_1);                     //+ Breite der 1-er Linien
                newHeight = newHeight + ((Originalbild.Height - 1) / 5) * (Linie_5 - Linie_1);     //+ Breite der 5-er Linien  minus breite der wegfallenden 1er linien
                newHeight = newHeight + ((Originalbild.Height - 1) / 10) * (Linie_10 - Linie_5);   //+ Breite der 10-er Linien  minus breite der wegfallenden 5er linien
                newHeight = newHeight + (Linie_10 * 2);                                            //+Platz für Bildumrandung in Dicke der 10er Linien
            }


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

                //Falls horizontales Raster eingestellt ist, die entsprechende Linie freilassen...
                if (checkBox_Rasterbild_horizontal_auch.Checked)
                {
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
                }
            }

            //Originalbild als bmp ablegen
            string filename = Bildtitel + "_Dateien/" + Bildtitel + "_Originalbild.bmp";
            Originalbild.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);

            //generiertes Rasterbild speichern

            //erste Version als bmp für maximale Qualitaet
            filename = Bildtitel + "_Dateien/" + Bildtitel + "_Rasterbild.bmp";
            Rasterbild.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);

            //zweite Version als png für weniger Speicherbedarf
            filename = Bildtitel + "_Dateien/" + Bildtitel + "_Rasterbild.png";
            Rasterbild.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            //Rasterbild mit Viewer öffnen
            //Process.Start(filename);
        }

        private void createPDF()
        {


            // Create a temporary file

            //string filename = String.Format(  Bildtitel + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +   ".pdf");
            string filename = String.Format(Bildtitel + "_Dateien/" + Bildtitel + ".pdf");
            myPDF = new PdfDocument();
            myPDF.Info.Title = "Häkelvorlage - " + Bildtitel;
            myPDF.Info.Author = "Denise die Wollmaus";
            myPDF.Info.Subject = "Crocheting Pattern";
            myPDF.Info.Keywords = "XGraphics";

            ////////// Titelseite erstellen ////////////
            PdfPage page = myPDF.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawTitle(page, gfx, Bildtitel);


            XRect rect_untertitel = new XRect(new XPoint(), gfx.PageSize);
            rect_untertitel.Inflate(-10, -40);

            XFont font_untertitel = new XFont("Times", 12, XFontStyle.Bold);
            gfx.DrawString("(" + Originalbild.Height + " Zeilen und " + Originalbild.Width + " Spalten)", font_untertitel, XBrushes.Black, rect_untertitel, XStringFormats.TopCenter);

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
                numericUpDown1.Value = i;
                selectedLine = Originalbild.Height - (int)numericUpDown1.Value;
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

                            DrawTitle(page, gfx, "Zeile " + i + " -  Seite " + SeitenZahl);
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
                    DrawTitle(page, gfx, "Zeile " + i + " -  Seite " + SeitenZahl);
                }
                else
                {
                    DrawTitle(page, gfx, "Zeile " + i);
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
                numericUpDown1.Value = Originalbild.Height - (int)((double)e.Y / getZoomFactorY());
                selectedLine = Originalbild.Height - (int)numericUpDown1.Value;
                Zeile_Auswerten(selectedLine);
            }
        }
    }
}
