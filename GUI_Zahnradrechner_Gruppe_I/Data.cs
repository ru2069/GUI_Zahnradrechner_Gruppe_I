using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Zahnradrechner_Gruppe_I
{
    public class Data
    {
        double modul;
        double zähnezahl;
        double kreiszahl = Math.PI;
        double kopfspielzahl = 0.167;
        double normeingriffswinkel = 20 * Math.PI / 180;
        double breite;
        double material;
        double schr;

        //Inputs
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

    }
}
