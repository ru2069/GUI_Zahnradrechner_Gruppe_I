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

        public void ErzeugeProfil(Data dat)
        {
            //Nullpunkt
            double x0 = 0;
            double y0 = 0;

            //Hilfsgrößen von Wilkos PDF
            double teilkreisradius = dat.getTeilkreisdurchmesser() / 2;
            double hilfskreisradius = teilkreisradius * 0.94;
            double fußkreisradius = teilkreisradius - (1.25 * dat.getModul());
            double kopfkreisradius = teilkreisradius + dat.getModul();
            double verrundungsradius = 0.35 * dat.getModul();

            double alpha = 20;
            double beta = 90 / dat.getZähnezahl();
            double betarad = Math.PI * beta / 180;
            double gamma = 90 - (alpha - beta);
            double gammarad = Math.PI * gamma / 180;
            double totalangle = 360.0 / dat.getZähnezahl();
            double totalanglerad = Math.PI * totalangle / 180;

            //Punkte
            //LinkerEvolKreis Mittelp. Koordinaten
            double mittelpunktEvol_x = hilfskreisradius * Math.Cos(gammarad);
            double mittelpunktEvol_y = hilfskreisradius * Math.Sin(gammarad);

            // Schnittpkt. auf Evolvente und Teilkreisradius
            double punktEvol_x = -teilkreisradius * Math.Sin(betarad);
            double punktEvol_y = teilkreisradius * Math.Cos(betarad);

            //Evolventenkreis Radius
            double EvolventenkreisRadius = Math.Sqrt(Math.Pow((mittelpunktEvol_x - punktEvol_x), 2) + Math.Pow((mittelpunktEvol_y - punktEvol_y), 2));

            //Koordinaten Schnittpunkt Kopfkreis und Evolventenkreis
            double evolventenKopfk_x = Schnittpunkt_X(x0, y0, kopfkreisradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius);
            double evolventenKopfk_y = Schnittpunkt_Y(x0, y0, kopfkreisradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius);

            //Mittelpunktkoordinaten Verrundung
            double mittelpunktVer_x = Schnittpunkt_X(x0, y0, fußkreisradius + verrundungsradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius + verrundungsradius);
            double mittelpunktVer_y = Schnittpunkt_Y(x0, y0, fußkreisradius + verrundungsradius, mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius + verrundungsradius);

            //Schnittpubktkoordinaten Verrundung - Evolventenkreis
            double evolventenVer_x = Schnittpunkt_X(mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);
            double evolventenVer_y = Schnittpunkt_Y(mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);

            //Schnittpunktkoordinaten Verrundung - Fußkreis
            double fußkreisVer_x = Schnittpunkt_X(x0, y0, fußkreisradius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);
            double fußkreisVer_y = Schnittpunkt_Y(x0, y0, fußkreisradius, mittelpunktVer_x, mittelpunktVer_y, verrundungsradius);

            //Koordinaten Anfangspunkt Fußkreis
            double hilfswinkel = totalanglerad - Math.Atan(Math.Abs(fußkreisVer_x) / Math.Abs(fußkreisVer_y));
            double anfangspunktFußk_x = -fußkreisradius * Math.Sin(hilfswinkel);
            double anfangspunktFußk_y = fußkreisradius * Math.Cos(hilfswinkel);

            //Skizze umbenennen und öffnen
            hsp_catiaProfil.set_Name("Außenverzahntes Stirnrad");
            Factory2D catfactory2D1 = hsp_catiaProfil.OpenEdition();

            //Nun die Punkte in die Skizze
            Point2D point_Ursprung = catfactory2D1.CreatePoint(x0, y0);
            Point2D pointAnfangFußkreis = catfactory2D1.CreatePoint(anfangspunktFußk_x, anfangspunktFußk_y);
            Point2D pointFußkreisVer_l = catfactory2D1.CreatePoint(fußkreisVer_x, fußkreisVer_y);
            Point2D pointFußkreisVer_r = catfactory2D1.CreatePoint(-fußkreisVer_x, fußkreisVer_y);
            Point2D pointMittelpunktVer_l = catfactory2D1.CreatePoint(mittelpunktVer_x, mittelpunktVer_y);
            Point2D pointMittelpunktVer_r = catfactory2D1.CreatePoint(-mittelpunktVer_x, mittelpunktVer_y);
            Point2D pointVerrundungEvol_l = catfactory2D1.CreatePoint(evolventenVer_x, evolventenVer_y);
            Point2D pointVerrundungEvol_r = catfactory2D1.CreatePoint(-evolventenVer_x, evolventenVer_y);
            Point2D pointMittelpunktevol_l = catfactory2D1.CreatePoint(mittelpunktEvol_x, mittelpunktEvol_y);
            Point2D pointMittelpunktevol_r = catfactory2D1.CreatePoint(-mittelpunktEvol_x, mittelpunktEvol_y);
            Point2D pointEvolventenKopfkreis_l = catfactory2D1.CreatePoint(evolventenKopfk_x, evolventenKopfk_y);
            Point2D pointEvolventenKopfkreis_r = catfactory2D1.CreatePoint(-evolventenKopfk_x, evolventenKopfk_y);

            //Kreise
            Circle2D kreisFußkreis = catfactory2D1.CreateCircle(x0, y0, fußkreisradius, 0, Math.PI * 2);
            kreisFußkreis.CenterPoint = point_Ursprung;
            kreisFußkreis.StartPoint = pointFußkreisVer_l;
            kreisFußkreis.EndPoint = pointAnfangFußkreis;

            Circle2D kreisVerrundung_l = catfactory2D1.CreateCircle(mittelpunktVer_x, mittelpunktVer_y, verrundungsradius, 0, Math.PI * 2);
            kreisVerrundung_l.CenterPoint = pointMittelpunktVer_l;
            kreisVerrundung_l.StartPoint = pointFußkreisVer_l;
            kreisVerrundung_l.EndPoint = pointVerrundungEvol_l;

            Circle2D kreisEvolventenkreis_l = catfactory2D1.CreateCircle(mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, 0, Math.PI * 2);
            kreisEvolventenkreis_l.CenterPoint = pointMittelpunktevol_l;
            kreisEvolventenkreis_l.StartPoint = pointEvolventenKopfkreis_l;
            kreisEvolventenkreis_l.EndPoint = pointVerrundungEvol_l;

            Circle2D kreisKopfkreis = catfactory2D1.CreateCircle(x0, y0, kopfkreisradius, 0, Math.PI * 2);
            kreisKopfkreis.CenterPoint = point_Ursprung;
            kreisKopfkreis.StartPoint = pointEvolventenKopfkreis_r;
            kreisKopfkreis.EndPoint = pointEvolventenKopfkreis_l;

            Circle2D kreisEvolventenkreis_r = catfactory2D1.CreateCircle(-mittelpunktEvol_x, mittelpunktEvol_y, EvolventenkreisRadius, 0, Math.PI * 2);
            kreisEvolventenkreis_r.CenterPoint = pointMittelpunktevol_r;
            kreisEvolventenkreis_r.StartPoint = pointVerrundungEvol_r;
            kreisEvolventenkreis_r.EndPoint = pointEvolventenKopfkreis_r;

            Circle2D kreisVerrundung_r = catfactory2D1.CreateCircle(-mittelpunktVer_x, mittelpunktVer_y, verrundungsradius, 0, Math.PI * 2);
            kreisVerrundung_r.CenterPoint = pointMittelpunktVer_r;
            kreisVerrundung_r.StartPoint = pointVerrundungEvol_r;
            kreisVerrundung_r.EndPoint = pointFußkreisVer_r;



            hsp_catiaProfil.CloseEdition();

            hsp_catiaPart.Part.Update();
        }

        public void ErzeugeDasNeueKreismuster(Data dat)
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
            angle1.Value = Convert.ToDouble(360 / dat.getZähnezahl());
            AngularRepartition angularRepartition2 = kreismuster.AngularRepartition;
            IntParam intParam1 = angularRepartition2.InstancesCount;
            intParam1.Value = Convert.ToInt32(dat.getZähnezahl()) + 1;


            //Kreismusterenden verbinden

            Reference refKreismuster = hsp_catiaPart.Part.CreateReferenceFromObject(kreismuster);
            HybridShapeAssemble verbindung = hybridShapeFactory1.AddNewJoin(refKreismuster, refKreismuster);
            Reference refVerbindung = hsp_catiaPart.Part.CreateReferenceFromObject(verbindung);

            hybridShapeFactory1.GSMVisibility(refVerbindung, 0);

            hsp_catiaPart.Part.MainBody.InsertHybridShape(verbindung);



            hsp_catiaPart.Part.Update();

            ErzeugedenNeuenBlock(refVerbindung, shapeFactory1, dat);
        }

        public void ErzeugedenNeuenBlock(Reference refVerbindung, ShapeFactory sf1, Data dat)
        {
            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;
            Pad catPad1 = sf1.AddNewPadFromRef(refVerbindung, dat.getBreite());



            hsp_catiaPart.Part.Update();
        }




        private double Schnittpunkt_X(double mittelpunkt_x, double mittelpunkt_y, double radius1, double mittelpunkt2_x, double mittelpunkt2_y, double radius2)
        {
            double d = Math.Sqrt(Math.Pow((mittelpunkt_x - mittelpunkt2_x), 2) + Math.Pow((mittelpunkt_y - mittelpunkt2_y), 2));
            double l = (Math.Pow(radius1, 2) - Math.Pow(radius2, 2) + Math.Pow(d, 2)) / (d * 2);
            double h;
            double epsilon = 0.00001;

            if (radius1 - l < -epsilon)
            {
                MessageBox.Show("Fehler");
            }
            if (Math.Abs(radius1 - l) < epsilon)
            {
                h = 0;
            }
            else
            {
                h = Math.Sqrt(Math.Pow(radius1, 2) - Math.Pow(l, 2));
            }

            return l * (mittelpunkt2_x - mittelpunkt_x) / d - h * (mittelpunkt2_y - mittelpunkt_y) / d + mittelpunkt_x;
        }

        private double Schnittpunkt_Y(double mittelpunkt_x, double mittelpunkt_y, double radius1, double mittelpunkt2_x, double mittelpunkt2_y, double Radius2)
        {
            double d = Math.Sqrt(Math.Pow((mittelpunkt_x - mittelpunkt2_x), 2) + Math.Pow((mittelpunkt_y - mittelpunkt2_y), 2));
            double l = (Math.Pow(radius1, 2) - Math.Pow(Radius2, 2) + Math.Pow(d, 2)) / (d * 2);
            double h;
            double epsilon = 0.00001;

            if (radius1 - l < -epsilon)
            {
                MessageBox.Show("Fehler");
            }
            if (Math.Abs(radius1 - l) < epsilon)
            {
                h = 0;
            }
            else
            {
                h = Math.Sqrt(Math.Pow(radius1, 2) - Math.Pow(l, 2));
            }

            return l * (mittelpunkt2_y - mittelpunkt_y) / d + h * (mittelpunkt2_x - mittelpunkt_x) / d + mittelpunkt_y;
        }
    }
}
