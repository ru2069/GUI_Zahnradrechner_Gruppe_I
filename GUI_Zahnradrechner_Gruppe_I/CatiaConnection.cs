using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using INFITF;
using MECMOD;
using PARTITF;

namespace GUI_Zahnradrechner_Gruppe_I
{
    class CatiaConnection
    {
        INFITF.Application hsp_catiaApp;
        MECMOD.PartDocument hsp_catiaPart;
        MECMOD.Sketch hsp_catiaProfil;

        public bool CATIALaeuft()
        {
            try
            {
                object catiaObject = System.Runtime.InteropServices.Marshal.GetActiveObject(
                    "CATIA.Application");
                hsp_catiaApp = (INFITF.Application)catiaObject;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean ErzeugePart()
        {
            INFITF.Documents catDocuments1 = hsp_catiaApp.Documents;
            hsp_catiaPart = catDocuments1.Add("Part") as MECMOD.PartDocument;
            return true;
        }

        public void ErstelleLeereSkizze()
        {
            // geometrisches Set auswaehlen und umbenennen
            HybridBodies catHybridBodies1 = hsp_catiaPart.Part.HybridBodies;
            HybridBody catHybridBody1;
            try
            {
                catHybridBody1 = catHybridBodies1.Item("Geometrisches Set.1");
            }
            catch (Exception)
            {
                MessageBox.Show("Kein geometrisches Set gefunden! " + Environment.NewLine +
                    "Ein PART manuell erzeugen und ein darauf achten, dass 'Geometisches Set' aktiviert ist.",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            catHybridBody1.set_Name("Profile");
            // neue Skizze im ausgewaehlten geometrischen Set anlegen
            Sketches catSketches1 = catHybridBody1.HybridSketches;
            OriginElements catOriginElements = hsp_catiaPart.Part.OriginElements;
            Reference catReference1 = (Reference)catOriginElements.PlaneYZ;
            hsp_catiaProfil = catSketches1.Add(catReference1);

            // Achsensystem in Skizze erstellen 
            ErzeugeAchsensystem();

            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }

        private void ErzeugeAchsensystem()
        {
            object[] arr = new object[] {0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0,
                                         0.0, 0.0, 1.0 };
            hsp_catiaProfil.SetAbsoluteAxisData(arr);
        }

        public void ErzeugeKontur(Double b, Double h)
        {
            // Skizze umbenennen
            hsp_catiaProfil.set_Name("ZR_Sketch");

            // Kreis in Skizze einzeichnen
            // Skizze oeffnen
            Factory2D catFactory2D1 = hsp_catiaProfil.OpenEdition();

            // Kreis erzeugen

            // erst die Punkte

            int n = Convert.ToInt32(b);
            double radius = h;
            double alpha = 2 * Math.PI / n;
            double[] kreisPx = new double[n];
            double[] kreisPy = new double[n];
            Point2D[] catPoints = new Point2D[n];

            for (int ii = 0; ii < n; ii++)
            {
                kreisPx[ii] = Math.Cos(alpha * ii) * radius;
                kreisPy[ii] = Math.Sin(alpha * ii) * radius;

                catPoints[ii] = catFactory2D1.CreatePoint(kreisPx[ii], kreisPy[ii]);
            }

            // dann die Linien
            Line2D[] catLines = new Line2D[n];
            for (int ii = 1; ii < n; ii++) // Achtung, mit 1 starten wg. -1
            {
                catLines[ii] = catFactory2D1.CreateLine(kreisPx[ii - 1], kreisPy[ii - 1], kreisPx[ii], kreisPy[ii]);
                catLines[ii].StartPoint = catPoints[ii - 1];
                catLines[ii].EndPoint = catPoints[ii];
            }
            // Jetzt noch mit erste Linie [0] den Kreis schliessen
            catLines[0] = catFactory2D1.CreateLine(kreisPx[n - 1], kreisPy[n - 1], kreisPx[0], kreisPy[0]);
            catLines[0].StartPoint = catPoints[n - 1];
            catLines[0].EndPoint = catPoints[0];

            // Skizzierer verlassen
            hsp_catiaProfil.CloseEdition();
            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }

        public void ErzeugeZahnrad(Double b)
        {
            // Hauptkoerper in Bearbeitung definieren
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;

            // Block(Balken) erzeugen
            ShapeFactory catShapeFactory1 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;
            Pad catPad1 = catShapeFactory1.AddNewPad(hsp_catiaProfil, b);

            // Block umbenennen
            catPad1.set_Name("Zahnrad");

            // Part aktualisieren
            hsp_catiaPart.Part.Update();
        }



    }
}
