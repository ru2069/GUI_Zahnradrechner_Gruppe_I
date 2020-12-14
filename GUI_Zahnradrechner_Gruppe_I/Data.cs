using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Zahnradrechner_Gruppe_I
{
    public class Data
    {
        double teilkreisdurchmesser;
        double modul;
        double zähnezahl;
        double kreiszahl = Math.PI;
        double kopfspielzahl = 0.167;
        double normeingriffswinkel = 20 * Math.PI / 180;
        double breite;
        double material;
        double schr;
        double bohrung;
        public double Bohrungsauswahl;
        public double bohrungsradius;
        public double PassfederBreite;
        public double PassfederHöhe;


        //Inputs
        public double getTeilkreisdurchmesser()
        {
            return teilkreisdurchmesser;
        }

        public void setTeilkreisdurchmesser(double teilkreisdurchmesser)
        {
            this.teilkreisdurchmesser = teilkreisdurchmesser;
        }
        //Modul

        public double getModul()
        {
            return modul;
        }

        public void setModul(double modul)
        {
            this.modul = modul;
        }

        //Zähnezahl

        public double getZähnezahl()
        {
            return zähnezahl;
        }

        public void setZähnezahl(double zähnezahl)
        {
            this.zähnezahl = zähnezahl;
        }

        //Breite

        public double getBreite()
        {
            return breite;
        }

        public void setBreite(double breite)
        {
            this.breite = breite;
        }

        //Material

        public double getMaterial()
        {
            return material;
        }

        public void setMaterial(double material)
        {
            this.material = material;
        }

        //Schrägungswinkel

        public double getSchrägungswinkel()
        {
            double schrDegree;
            schrDegree = schr * Math.PI / 180;
            return schrDegree;
        }

        public void setSchrägungswinkel(double schr)
        {
            this.schr = schr;
        }

        public double getBohrung()
        {
            double bohrungsradius = bohrung / 2;
            return bohrungsradius;
        }

        public void setBohrung(double bohrung)
        {
            this.bohrung = bohrung;
        }


        //Konstanten
        //Kreiszahl

        public double getKreiszahl()
        {
            return kreiszahl;
        }

        //Kopspielzahl

        public double getKopfspielzahl()
        {
            return kopfspielzahl;
        }

        //Normeingriffswinkel

        public double getNormeingriffswinkel()
        {
            return normeingriffswinkel;
        }

        public double getPassfederbreite()
        {
            if (bohrung <= 12)
            {
                PassfederBreite = 4;
            }
            if (bohrung > 12 && bohrung <= 17)
            {
                PassfederBreite = 5;
            }
            if (bohrung > 17 && bohrung <= 22)
            {
                PassfederBreite = 6;
            }
            if (bohrung > 22 && bohrung <= 30)
            {
                PassfederBreite = 8;
            }
            if (bohrung > 30 && bohrung <= 38)
            {
                PassfederBreite = 10;
            }
            if (bohrung > 38 && bohrung <= 44)
            {
                PassfederBreite = 12;
            }
            if (bohrung > 44 && bohrung <= 50)
            {
                PassfederBreite = 14;
            }
            if (bohrung > 50 && bohrung <= 58)
            {
                PassfederBreite = 16;
            }
            if (bohrung > 58 && bohrung <= 65)
            {
                PassfederBreite = 18;
            }
            if (bohrung > 65 && bohrung <= 75)
            {
                PassfederBreite = 20;
            }
            if (bohrung > 75 && bohrung <= 85)
            {
                PassfederBreite = 22;
            }
            if (bohrung > 85 && bohrung <= 95)
            {
                PassfederBreite = 25;
            }
            if (bohrung > 95 && bohrung <= 110)
            {
                PassfederBreite = 28;
            }
            if (bohrung > 110 && bohrung <= 130)
            {
                PassfederBreite = 32;
            }
            if (bohrung > 130 && bohrung <= 150)
            {
                PassfederBreite = 36;
            }
            if (bohrung > 150 && bohrung <= 170)
            {
                PassfederBreite = 40;
            }
            if (bohrung > 170)
            {
                PassfederBreite = 45;
            }

            return PassfederBreite;
        }

        public double getPassfederhöhe()
        {
            if (bohrung <= 12)
            {
                PassfederHöhe = getBohrung() + 1.8;
            }
            if (bohrung > 12 && bohrung <= 17)
            {
                PassfederHöhe = getBohrung() + 2.3;
            }
            if (bohrung > 17 && bohrung <= 22)
            {
                PassfederHöhe = getBohrung() + 2.8;
            }
            if (bohrung > 22 && bohrung <= 44)
            {
                PassfederHöhe = getBohrung() + 3.3;
            }
            if (bohrung > 44 && bohrung <= 50)
            {
                PassfederHöhe = getBohrung() + 3.8;
            }
            if (bohrung > 50 && bohrung <= 58)
            {
                PassfederHöhe = getBohrung() + 4.3;
            }
            if (bohrung > 58 && bohrung <= 65)
            {
                PassfederHöhe = getBohrung() + 4.4;
            }
            if (bohrung > 65 && bohrung <= 75)
            {
                PassfederHöhe = getBohrung() + 4.9;
            }
            if (bohrung > 75 && bohrung <= 95)
            {
                PassfederHöhe = getBohrung() + 5.4;
            }
            if (bohrung > 95 && bohrung <= 110)
            {
                PassfederHöhe = getBohrung() + 6.4;
            }
            if (bohrung > 110 && bohrung <= 130)
            {
                PassfederHöhe = getBohrung() + 7.4;
            }
            if (bohrung > 130 && bohrung <= 150)
            {
                PassfederHöhe = getBohrung() + 8.4;
            }
            if (bohrung > 150 && bohrung <= 170)
            {
                PassfederHöhe = getBohrung() + 9.4;
            }
            if (bohrung > 170)
            {
                PassfederHöhe = getBohrung() + 10.4;
            }
            return PassfederHöhe;
        }
    }
}