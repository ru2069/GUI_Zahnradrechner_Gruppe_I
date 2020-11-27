using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Zahnradrechner_Gruppe_I
{
    public class Berechnungen
    {
        //Berechnungen
        public double Teilkreisdurchmesser_d(double modul, double zähnezahl)
        {
            double d = modul * zähnezahl;
            return d;
        }
        public double Teilung_p(double kreiszahl, double modul)
        {
            double teilung = kreiszahl * modul;
            return teilung;
        }
        public double Zahnkopfhöhe_ha(double modul)
        {
            double zahnkopfhöhe = modul;
            return zahnkopfhöhe;
        }
        public double Kopfspiel_c(double modul, double kopfspielzahl)
        {
            double kopfspiel = kopfspielzahl * modul;
            return kopfspiel;
        }
        public double Zahnfußhöhe_hf(double modul, double kopfspiel)
        {
            double zahnfußhöhe = modul + kopfspiel;
            return zahnfußhöhe;
        }
        public double Zahnhöhe_h(double modul, double kopfspiel)
        {
            double zahnhöhe = (2 * modul) + kopfspiel;
            return zahnhöhe;
        }
        public double Kopfkreisdurchmesser_daa(double d, double modul)
        {
            double außenKopfkreisdurchmesser = d + 2 * modul;
            return außenKopfkreisdurchmesser;
        }
        public double Fußkreisdurchmesser_dfa(double d, double modul, double kopfspiel)
        {
            double außenFußkreisdurchmesser = d - 2 * (modul + kopfspiel);
            return außenFußkreisdurchmesser;
        }
        public double Kopfkreisdurchmesser_dai(double d, double modul)
        {
            double innenKopfkreisdurchmesser = d - 2 * modul;
            return innenKopfkreisdurchmesser;
        }
        public double Fußkreisdurchmesser_dfi(double d, double modul, double kopfspiel)
        {
            double innenFußkreisdurchmesser = d + 2 * (modul + kopfspiel);
            return innenFußkreisdurchmesser;
        }
        public double Grundkreisdurchmesser_db(double d, double normeingriffswinkel)
        {
            double db = d * Math.Cos(normeingriffswinkel);
            return db;
        }
        public double Volumen(double außenKopfkreisdurchmesser, double Kreiszahl, double breite)
        {
            double v = ((Kreiszahl * Math.Pow(außenKopfkreisdurchmesser, 2) / 4) * breite) / 1000;
            return v;
        }
        public double Masse(double material, double v)
        {
            double mg = material * v;
            return mg;
        }


        //Berechnungen Schrägverzahnung
        public double normalteilung_pn(double kreiszahl, double modul)
        {
            double pn = kreiszahl * modul;
            return pn;
        }
        public double stirnmodul_mt(double modul, double schrDegree)
        {
            double mt = modul / Math.Cos(schrDegree);
            return mt;
        }
        public double stirnteilung_pt(double normalteilung, double schrDegree)
        {
            double pt = normalteilung / Math.Cos(schrDegree);
            return pt;
        }
        public double schrägTeilkreisdurchmesser_d(double stirnmodul, double zähnezahl)
        {
            double d = stirnmodul * zähnezahl;
            return d;
        }
    }
}