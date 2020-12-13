using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using INFITF;
using MECMOD;
using PARTITF;
using HybridShapeTypeLib;
using KnowledgewareTypeLib;
using ProductStructureTypeLib;

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

        public void ErzeugeProfil(double d, double m, double z)
        {
            //Nullpunkt
            double x0 = 0;
            double y0 = 0;

            //Hilfsgrößen von Wilkos PDF
            double Teilkreisradius = d / 2;
            double Hilfskreisradius = Teilkreisradius * 0.94;
            double Fußkreisradius = Teilkreisradius - (1.25 * m);
            double Kopfkreisradius = Teilkreisradius + m;
            double Verrundungsradius = 0.35 * m;

            double Alpha = 20;
            double Beta = 90 / z;
            double Betarad = Math.PI * Beta / 180;
            double Gamma = 90 - (Alpha - Beta);
            double Gammarad = Math.PI * Gamma / 180;
            double Totalangle = 360.0 / z;
            double Totalanglerad = Math.PI * Totalangle / 180;

            //Punkte
            //LinkerEvolKreis Mittelp. Koordinaten
            double xMittelpunktaufEvol_links = Hilfskreisradius * Math.Cos(Gammarad);
            double yMittelpunktaufEvol_links = Hilfskreisradius * Math.Sin(Gammarad);

            // Schnittpkt. auf Evolvente und Teilkreisradius
            double xPunktAufEvolvente = -Teilkreisradius * Math.Sin(Betarad);
            double yPunktAufEvolvente = Teilkreisradius * Math.Cos(Betarad);

            //Evolventenkreis Radius
            double EvolventenkreisRadius = Math.Sqrt(Math.Pow((xMittelpunktaufEvol_links - xPunktAufEvolvente), 2) + Math.Pow((yMittelpunktaufEvol_links - yPunktAufEvolvente), 2));

            //Koordinaten Schnittpunkt Kopfkreis und Evolventenkreis
            double xEvolventenkopfkreis_links = Schnittpunkt_X(x0, y0, Kopfkreisradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius);
            double yEvolventenkopfkreis_links = Schnittpunkt_Y(x0, y0, Kopfkreisradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius);

            //Mittelpunktkoordinaten Verrundung
            double xMittelpunktVerrundung_links = Schnittpunkt_X(x0, y0, Fußkreisradius + Verrundungsradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius + Verrundungsradius);
            double yMittelpunktVerrundung_links = Schnittpunkt_Y(x0, y0, Fußkreisradius + Verrundungsradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius + Verrundungsradius);

            //Schnittpubktkoordinaten Verrundung - Evolventenkreis
            double x_SP_EvolventeVerrundung_links = Schnittpunkt_X(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);
            double y_SP_EvolventeVerrundung_links = Schnittpunkt_Y(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);

            //Schnittpunktkoordinaten Verrundung - Fußkreis
            double x_SP_FußkreisradiusVerrundung_links = Schnittpunkt_X(x0, y0, Fußkreisradius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);
            double y_SP_FußkreisradiusVerrundung_links = Schnittpunkt_Y(x0, y0, Fußkreisradius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);

            //Koordinaten Anfangspunkt Fußkreis
            double Hilfswinkel = Totalanglerad - Math.Atan(Math.Abs(x_SP_FußkreisradiusVerrundung_links) / Math.Abs(y_SP_FußkreisradiusVerrundung_links));
            double x_AnfangspunktFußkreis = -Fußkreisradius * Math.Sin(Hilfswinkel);
            double y_AnfangspunktFußkreis = Fußkreisradius * Math.Cos(Hilfswinkel);

            //Skizze umbenennen und öffnen
            hsp_catiaProfil.set_Name("Außenverzahntes Stirnrad");
            Factory2D catfactory2D1 = hsp_catiaProfil.OpenEdition();

            //Nun die Punkte in die Skizze
            Point2D point_Ursprung = catfactory2D1.CreatePoint(x0, y0);
            Point2D pointAnfangFußkreisLinks = catfactory2D1.CreatePoint(x_AnfangspunktFußkreis, y_AnfangspunktFußkreis);
            Point2D pointFußkreisVerrundungLinks = catfactory2D1.CreatePoint(x_SP_FußkreisradiusVerrundung_links, y_SP_FußkreisradiusVerrundung_links);
            Point2D pointFußkreisVerrundungRechts = catfactory2D1.CreatePoint(-x_SP_FußkreisradiusVerrundung_links, y_SP_FußkreisradiusVerrundung_links);
            Point2D pointMittelpunktVerrundungLinks = catfactory2D1.CreatePoint(xMittelpunktVerrundung_links, yMittelpunktVerrundung_links);
            Point2D pointMittelpunktVerrundungRechts = catfactory2D1.CreatePoint(-xMittelpunktVerrundung_links, yMittelpunktVerrundung_links);
            Point2D pointVerrundungEvolventeLinks = catfactory2D1.CreatePoint(x_SP_EvolventeVerrundung_links, y_SP_EvolventeVerrundung_links);
            Point2D pointVerrundungEvolventeRechts = catfactory2D1.CreatePoint(-x_SP_EvolventeVerrundung_links, y_SP_EvolventeVerrundung_links);
            Point2D pointMittelpunktevolventeLinks = catfactory2D1.CreatePoint(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links);
            Point2D pointMittelpunktevolventeRechts = catfactory2D1.CreatePoint(-xMittelpunktaufEvol_links, yMittelpunktaufEvol_links);
            Point2D pointEvolventenKopfkreisLinks = catfactory2D1.CreatePoint(xEvolventenkopfkreis_links, yEvolventenkopfkreis_links);
            Point2D pointEvolventenKopfkreisRechts = catfactory2D1.CreatePoint(-xEvolventenkopfkreis_links, yEvolventenkopfkreis_links);

            //Kreise
            Circle2D KreisFrußkreis = catfactory2D1.CreateCircle(x0, y0, Fußkreisradius, 0, Math.PI * 2);
            KreisFrußkreis.CenterPoint = point_Ursprung;
            KreisFrußkreis.StartPoint = pointFußkreisVerrundungLinks;
            KreisFrußkreis.EndPoint = pointAnfangFußkreisLinks;

            Circle2D KreisVerrundungLinks = catfactory2D1.CreateCircle(xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius, 0, Math.PI * 2);
            KreisVerrundungLinks.CenterPoint = pointMittelpunktVerrundungLinks;
            KreisVerrundungLinks.StartPoint = pointFußkreisVerrundungLinks;
            KreisVerrundungLinks.EndPoint = pointVerrundungEvolventeLinks;

            Circle2D KreisEvolventenkreisLinks = catfactory2D1.CreateCircle(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, 0, Math.PI * 2);
            KreisEvolventenkreisLinks.CenterPoint = pointMittelpunktevolventeLinks;
            KreisEvolventenkreisLinks.StartPoint = pointEvolventenKopfkreisLinks;
            KreisEvolventenkreisLinks.EndPoint = pointVerrundungEvolventeLinks;

            Circle2D KreisKopfkreis = catfactory2D1.CreateCircle(x0, y0, Kopfkreisradius, 0, Math.PI * 2);
            KreisKopfkreis.CenterPoint = point_Ursprung;
            KreisKopfkreis.StartPoint = pointEvolventenKopfkreisRechts;
            KreisKopfkreis.EndPoint = pointEvolventenKopfkreisLinks;

            Circle2D KreisEvolventenkreisRechts = catfactory2D1.CreateCircle(-xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, 0, Math.PI * 2);
            KreisEvolventenkreisRechts.CenterPoint = pointMittelpunktevolventeRechts;
            KreisEvolventenkreisRechts.StartPoint = pointVerrundungEvolventeRechts;
            KreisEvolventenkreisRechts.EndPoint = pointEvolventenKopfkreisRechts;

            Circle2D KreisVerrundungRechts = catfactory2D1.CreateCircle(-xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius, 0, Math.PI * 2);
            KreisVerrundungRechts.CenterPoint = pointMittelpunktVerrundungRechts;
            KreisVerrundungRechts.StartPoint = pointVerrundungEvolventeRechts;
            KreisVerrundungRechts.EndPoint = pointFußkreisVerrundungRechts;



            hsp_catiaProfil.CloseEdition();

            hsp_catiaPart.Part.Update();
        }

        public void ErzeugeDasNeueKreismuster(double b, double z)
        {
            ShapeFactory shapeFactory1 = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;
            HybridShapeFactory hybridShapeFactory1 = (HybridShapeFactory)hsp_catiaPart.Part.HybridShapeFactory;

            Factory2D factory2D1 = hsp_catiaProfil.Factory2D;

            HybridShapePointCoord ursprung = hybridShapeFactory1.AddNewPointCoord(0, 0, 0);
            Reference refUrsprung = hsp_catiaPart.Part.CreateReferenceFromObject(ursprung);

            HybridShapeDirection xRichtung = hybridShapeFactory1.AddNewDirectionByCoord(1, 0, 0);
            Reference refxRichtung = hsp_catiaPart.Part.CreateReferenceFromObject(xRichtung);

            CircPattern kreismuster = shapeFactory1.AddNewSurfacicCircPattern(factory2D1, 1, 2, 0, 0, 1, 1, refUrsprung, refxRichtung, false, 0, true, false);
            kreismuster.CircularPatternParameters = CatCircularPatternParameters.catInstancesandAngularSpacing;
            AngularRepartition angularRepartition1 = kreismuster.AngularRepartition;
            Angle angle1 = angularRepartition1.AngularSpacing;
            angle1.Value = Convert.ToDouble(360 / z);
            AngularRepartition angularRepartition2 = kreismuster.AngularRepartition;
            IntParam intParam1 = angularRepartition2.InstancesCount;
            intParam1.Value = Convert.ToInt32(z) + 1;


            //Kreismusterenden verbinden

            Reference refKreismuster = hsp_catiaPart.Part.CreateReferenceFromObject(kreismuster);
            HybridShapeAssemble verbindung = hybridShapeFactory1.AddNewJoin(refKreismuster, refKreismuster);
            Reference refVerbindung = hsp_catiaPart.Part.CreateReferenceFromObject(verbindung);

            hybridShapeFactory1.GSMVisibility(refVerbindung, 0);

            hsp_catiaPart.Part.MainBody.InsertHybridShape(verbindung);



            hsp_catiaPart.Part.Update();

            ErzeugedenNeuenBlock(refVerbindung, shapeFactory1, b);
        }

        public void ErzeugedenNeuenBlock(Reference refVerbindung, ShapeFactory sf1, double b)
        {
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
            Pad catPad1 = sf1.AddNewPadFromRef(refVerbindung, b);



            hsp_catiaPart.Part.Update();
        }




        private double Schnittpunkt_X(double xMittelpunkt, double yMittelpunkt, double Radius1, double xMittelpunkt2, double yMittelpunkt2, double Radius2)
        {
            double d = Math.Sqrt(Math.Pow((xMittelpunkt - xMittelpunkt2), 2) + Math.Pow((yMittelpunkt - yMittelpunkt2), 2));
            double l = (Math.Pow(Radius1, 2) - Math.Pow(Radius2, 2) + Math.Pow(d, 2)) / (d * 2);
            double h;
            double epsilon = 0.00001;

            if (Radius1 - l < -epsilon)
            {
                MessageBox.Show("Fehler");
            }
            if (Math.Abs(Radius1 - l) < epsilon)
            {
                h = 0;
            }
            else
            {
                h = Math.Sqrt(Math.Pow(Radius1, 2) - Math.Pow(l, 2));
            }

            return l * (xMittelpunkt2 - xMittelpunkt) / d - h * (yMittelpunkt2 - yMittelpunkt) / d + xMittelpunkt;
        }

        private double Schnittpunkt_Y(double xMittelpunkt, double yMittelpunkt, double Radius1, double xMittelpunkt2, double yMittelpunkt2, double Radius2)
        {
            double d = Math.Sqrt(Math.Pow((xMittelpunkt - xMittelpunkt2), 2) + Math.Pow((yMittelpunkt - yMittelpunkt2), 2));
            double l = (Math.Pow(Radius1, 2) - Math.Pow(Radius2, 2) + Math.Pow(d, 2)) / (d * 2);
            double h;
            double epsilon = 0.00001;

            if (Radius1 - l < -epsilon)
            {
                MessageBox.Show("Fehler");
            }
            if (Math.Abs(Radius1 - l) < epsilon)
            {
                h = 0;
            }
            else
            {
                h = Math.Sqrt(Math.Pow(Radius1, 2) - Math.Pow(l, 2));
            }

            return l * (yMittelpunkt2 - yMittelpunkt) / d + h * (xMittelpunkt2 - xMittelpunkt) / d + yMittelpunkt;
        }
    }
}
