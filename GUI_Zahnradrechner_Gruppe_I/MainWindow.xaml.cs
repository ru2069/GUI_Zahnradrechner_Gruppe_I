using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace GUI_Zahnradrechner_Gruppe_I
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //AUßENVERZAHNUNG
        public void Btn_ClickAußen(object sender, RoutedEventArgs e)
        {
            Data dat = new Data();

            if (rdbtn_gerade.IsChecked == false && rdbtn_schräg.IsChecked == false)
            {
                MessageBox.Show("Bitte zwischen Gerad- oder Schrägverzahnung wählen!");
            }

            //If-Abfrage Radiobutton Gerade
            if (rdbtn_gerade.IsChecked == true)
            {
                //If-Abfragen Zahlcheck der Eingaben
                string zahlCheckModul = txb_modul_außen.Text;
                string zahlCheckZähnezahl = txb_zaehnezahl_außen.Text;
                string zahlCheckBreite = txb_breite_außen.Text;

                if (Eingabecheck(zahlCheckModul) == true)
                {
                    txb_modul_außen.Background = Brushes.White;

                    if (Eingabecheck(zahlCheckZähnezahl) == true)
                    {
                        txb_zaehnezahl_außen.Background = Brushes.White;

                        if (Eingabecheck(zahlCheckBreite) == true)
                        {
                            txb_breite_außen.Background = Brushes.White;

                            //BERECHNUNGEN
                            double m = Convert.ToDouble(txb_modul_außen.Text);
                            dat.setModul(m);
                            double z = Convert.ToDouble(txb_zaehnezahl_außen.Text);
                            dat.setZähnezahl(z);
                            double b = Convert.ToDouble(txb_breite_außen.Text);
                            dat.setBreite(b);
                            dat.setMaterial(material);

                            BerechnungenGeradeAußen(dat);
                        }
                        else if (Eingabecheck(zahlCheckBreite) == false)
                        {
                            MessageBox.Show("Sie müssen eine Zahl als Breite eingeben!");
                            txb_breite_außen.Background = Brushes.OrangeRed;
                        }
                    }
                    else if (Eingabecheck(zahlCheckZähnezahl) == false)
                    {
                        MessageBox.Show("Sie müssen eine Zahl als Zähnezahl eingeben!");
                        txb_zaehnezahl_außen.Background = Brushes.OrangeRed;
                    }
                }
                else if (Eingabecheck(zahlCheckModul) == false)
                {
                    MessageBox.Show("Sie müssen eine Zahl als Modul eingeben!");
                    txb_modul_außen.Background = Brushes.OrangeRed;
                }
            }

            //If-Abfrage Radiobutton Schräg
            else if (rdbtn_schräg.IsChecked == true)
            {
                //If-Abfragen Zahlcheck der Eingaben
                string zahlCheckModul = txb_modul_außen.Text;
                string zahlCheckZähnezahl = txb_zaehnezahl_außen.Text;
                string zahlCheckBreite = txb_breite_außen.Text;
                string zahlCheckSchrägungswinkel = txb_schraegungswinkel.Text;

                if (Eingabecheck(zahlCheckModul) == true)
                {
                    txb_modul_außen.Background = Brushes.White;

                    if (Eingabecheck(zahlCheckZähnezahl) == true)
                    {
                        txb_zaehnezahl_außen.Background = Brushes.White;
                        if (Eingabecheck(zahlCheckBreite) == true)
                        {
                            txb_breite_außen.Background = Brushes.White;
                            if (Eingabecheck(zahlCheckSchrägungswinkel) == true)
                            {
                                txb_schraegungswinkel.Background = Brushes.White;

                                //BERECHNUNGEN
                                double m = Convert.ToDouble(txb_modul_außen.Text);
                                dat.setModul(m);
                                double z = Convert.ToDouble(txb_zaehnezahl_außen.Text);
                                dat.setZähnezahl(z);
                                double b = Convert.ToDouble(txb_breite_außen.Text);
                                dat.setBreite(b);
                                double schr = Convert.ToDouble(txb_schraegungswinkel.Text);
                                dat.setSchrägungswinkel(schr);
                                dat.setMaterial(material);

                                BerechnungenSchrägAußen(dat);
                            }
                            else if (Eingabecheck(zahlCheckSchrägungswinkel) == false)
                            {
                                MessageBox.Show("Sie müssen eine Zahl als Schrägungswinkel eingeben!");
                                txb_schraegungswinkel.Background = Brushes.OrangeRed;
                            }
                        }
                        else if (Eingabecheck(zahlCheckBreite) == false)
                        {
                            MessageBox.Show("Sie müssen eine Zahl als Breite eingeben!");
                            txb_breite_außen.Background = Brushes.OrangeRed;
                        }
                    }
                    else if (Eingabecheck(zahlCheckZähnezahl) == false)
                    {
                        MessageBox.Show("Sie müssen eine Zahl als Zähnezahl eingeben!");
                        txb_zaehnezahl_außen.Background = Brushes.OrangeRed;
                    }
                }
                else if (Eingabecheck(zahlCheckModul) == false)
                {
                    MessageBox.Show("Sie müssen eine Zahl als Modul eingeben!");
                    txb_modul_außen.Background = Brushes.OrangeRed;
                }
            }
        }


        //INNENVERZAHNUNG
        public void Btn_ClickInnen(object sender, RoutedEventArgs e)
        {
            Data dat = new Data();

            //If-Abfragen Zahlcheck der Eingaben
            string zahlCheckModul = txb_modul_innen.Text;
            string zahlCheckZähnezahl = txb_zaehnezahl_innen.Text;
            string zahlCheckBreite = txb_breite_innen.Text;

            if (Eingabecheck(zahlCheckModul) == true)
            {
                txb_modul_innen.Background = Brushes.White;

                if (Eingabecheck(zahlCheckZähnezahl) == true)
                {
                    txb_zaehnezahl_innen.Background = Brushes.White;

                    if (Eingabecheck(zahlCheckBreite) == true)
                    {
                        txb_breite_innen.Background = Brushes.White;

                        //BERECHNUNGEN
                        double m = Convert.ToDouble(txb_modul_innen.Text);
                        dat.setModul(m);
                        double z = Convert.ToDouble(txb_zaehnezahl_innen.Text);
                        dat.setZähnezahl(z);
                        double b = Convert.ToDouble(txb_breite_innen.Text);
                        dat.setBreite(b);

                        //Berechnungen
                        BerechnungenGeradeInnen(dat);
                    }
                    else if (Eingabecheck(zahlCheckBreite) == false)
                    {
                        MessageBox.Show("Sie müssen eine Zahl als Breite eingeben!");
                        txb_breite_innen.Background = Brushes.OrangeRed;
                    }
                }
                else if (Eingabecheck(zahlCheckZähnezahl) == false)
                {
                    MessageBox.Show("Sie müssen eine Zahl als Zähnezahl eingeben!");
                    txb_zaehnezahl_innen.Background = Brushes.OrangeRed;
                }
            }
            else if (Eingabecheck(zahlCheckModul) == false)
            {
                MessageBox.Show("Sie müssen eine Zahl als Modul eingeben!");
                txb_modul_innen.Background = Brushes.OrangeRed;
            }
        }



        //Zahlcheck
        private bool Eingabecheck(string zahlCheck)
        {
            try
            {
                double eingabeZahl = double.Parse(zahlCheck);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        //Berechnungen
        private Data BerechnungenGeradeAußen(Data dat)
        {
            //If-Abfragen Korrekte Eingaben
            if (dat.getZähnezahl() % 1 == 0 && dat.getZähnezahl() >= 2 && dat.getModul() > 0 && dat.getBreite() > 0)
            {
                Berechnungen prg = new Berechnungen();

                txb_breite_außen.Background = Brushes.White;
                txb_zaehnezahl_außen.Background = Brushes.White;
                txb_modul_außen.Background = Brushes.White;
                int round = Convert.ToInt32(cmb_rundung.Text);


                double d = prg.Teilkreisdurchmesser_d(dat.getModul(), dat.getZähnezahl());
                dat.setTeilkreisdurchmesser(d);
                txb_teilkreisdurchmesser.Text = Convert.ToString(Math.Round(d, round) + " mm");

                double teilung = prg.Teilung_p(dat.getKreiszahl(), dat.getModul());
                txb_teilung.Text = Convert.ToString(Math.Round(teilung, round) + " mm");

                double kopfspiel = prg.Kopfspiel_c(dat.getModul(), dat.getKopfspielzahl());
                txb_kopfspiel.Text = Convert.ToString(Math.Round(kopfspiel, round) + " mm");

                double außenKopfkreisdurchmesser = prg.Kopfkreisdurchmesser_daa(d, dat.getModul());
                txb_kopfkreisdurchmesser.Text = Convert.ToString(Math.Round(außenKopfkreisdurchmesser, round) + " mm");

                double außenFußkreisdurchmesser = prg.Fußkreisdurchmesser_dfa(d, dat.getModul(), kopfspiel);
                txb_fußkreisdurchmesser.Text = Convert.ToString(Math.Round(außenFußkreisdurchmesser, round) + " mm");

                double zahnhöhe = prg.Zahnhöhe_h(dat.getModul(), kopfspiel);
                txb_zahnhoehe.Text = Convert.ToString(Math.Round(zahnhöhe, round) + " mm");

                double zahnkopfhöhe = prg.Zahnkopfhöhe_ha(dat.getModul());
                txb_zahnkopfhoehe.Text = Convert.ToString(Math.Round(zahnkopfhöhe, round) + " mm");

                double zahnfüßhöhe = prg.Zahnfußhöhe_hf(dat.getModul(), kopfspiel);
                txb_zahnfußhoehe.Text = Convert.ToString(Math.Round(zahnfüßhöhe, round) + " mm");

                double grundkreisdurchmesser = prg.Grundkreisdurchmesser_db(d, dat.getNormeingriffswinkel());
                txb_grundkreisdurchmesser.Text = Convert.ToString(Math.Round(grundkreisdurchmesser, round) + " mm");

                double volumen = prg.Volumen(außenKopfkreisdurchmesser, dat.getKreiszahl(), dat.getBreite());
                txb_volumen.Text = Convert.ToString(Math.Round(volumen, round) + " mm^3");

                double masse = prg.Masse(dat.getMaterial(), volumen);
                txb_masse.Text = Convert.ToString("~" + (Math.Round(masse, round)) + " g");

            }
            // Fehler: Falsche Werte
            else
            {
                if (dat.getZähnezahl() % 1 != 0)
                {
                    MessageBox.Show("Bitte eine ganzzahlige Zähnezahl über 2 eingeben!");
                    txb_zaehnezahl_außen.Background = Brushes.OrangeRed;
                }
                if (dat.getZähnezahl() < 2)
                {
                    MessageBox.Show("Bitte eine ganzzahlige Zähnezahl über 2 eingeben!");
                    txb_zaehnezahl_außen.Background = Brushes.OrangeRed;
                }

                if (dat.getModul() <= 0)
                {
                    MessageBox.Show("Bitte Modul über 0 wählen!");
                    txb_modul_außen.Background = Brushes.OrangeRed;
                }

                if (dat.getBreite() <= 0)
                {
                    MessageBox.Show("Bitte Breite über 0 wählen!");
                    txb_breite_außen.Background = Brushes.OrangeRed;
                }

            }
            return dat;
        }

        private Data BerechnungenSchrägAußen(Data dat)
        {
            //If-Abfragen Korrekte Eingaben
            if (dat.getZähnezahl() % 1 == 0 && dat.getZähnezahl() >= 2 && dat.getModul() > 0 && dat.getBreite() > 0 && dat.getSchrägungswinkel() > 0 && dat.getSchrägungswinkel() < 1.5704)
            {
                Berechnungen prg = new Berechnungen();

                txb_breite_außen.Background = Brushes.White;
                txb_zaehnezahl_außen.Background = Brushes.White;
                txb_modul_außen.Background = Brushes.White;
                txb_schraegungswinkel.Background = Brushes.White;
                int round = Convert.ToInt32(cmb_rundung.Text);


                double normalteilung = prg.normalteilung_pn(dat.getKreiszahl(), dat.getModul());
                txb_teilung.Text = Convert.ToString(Math.Round(normalteilung, round) + " mm");

                double stirnmodul = prg.stirnmodul_mt(dat.getModul(), dat.getSchrägungswinkel());
                txb_stirnmodul.Text = Convert.ToString(Math.Round(stirnmodul, round) + " mm");

                double stirnteilung = prg.stirnteilung_pt(normalteilung, dat.getSchrägungswinkel());
                txb_stirnteilung.Text = Convert.ToString(Math.Round(stirnteilung, round) + " mm");

                double d = prg.schrägTeilkreisdurchmesser_d(stirnmodul, dat.getZähnezahl());
                dat.setTeilkreisdurchmesser(d);
                txb_teilkreisdurchmesser.Text = Convert.ToString(Math.Round(d, round) + " mm");

                double teilung = prg.Teilung_p(dat.getKreiszahl(), dat.getModul());
                txb_teilung.Text = Convert.ToString(Math.Round(teilung, round) + " mm");

                double kopfspiel = prg.Kopfspiel_c(dat.getModul(), dat.getKopfspielzahl());
                txb_kopfspiel.Text = Convert.ToString(Math.Round(kopfspiel, round) + " mm");

                double außenKopfkreisdurchmesser = prg.Kopfkreisdurchmesser_daa(d, dat.getModul());
                txb_kopfkreisdurchmesser.Text = Convert.ToString(Math.Round(außenKopfkreisdurchmesser, round) + " mm");

                double außenFußkreisdurchmesser = prg.Fußkreisdurchmesser_dfa(d, dat.getModul(), kopfspiel);
                txb_fußkreisdurchmesser.Text = Convert.ToString(Math.Round(außenFußkreisdurchmesser, round) + " mm");

                double zahnhöhe = prg.Zahnhöhe_h(dat.getModul(), kopfspiel);
                txb_zahnhoehe.Text = Convert.ToString(Math.Round(zahnhöhe, round) + " mm");

                double zahnkopfhöhe = prg.Zahnkopfhöhe_ha(dat.getModul());
                txb_zahnkopfhoehe.Text = Convert.ToString(Math.Round(zahnkopfhöhe, round) + " mm");

                double grundkreisdurchmesser = prg.Grundkreisdurchmesser_db(d, dat.getNormeingriffswinkel());
                txb_grundkreisdurchmesser.Text = Convert.ToString(Math.Round(grundkreisdurchmesser, round) + " mm");

                double zahnfüßhöhe = prg.Zahnfußhöhe_hf(dat.getModul(), kopfspiel);
                txb_zahnfußhoehe.Text = Convert.ToString(Math.Round(zahnfüßhöhe, round) + " mm");

                double volumen = prg.Volumen(außenKopfkreisdurchmesser, dat.getKreiszahl(), dat.getBreite());
                txb_volumen.Text = Convert.ToString(Math.Round(volumen, round) + " mm^3");

                double masse = prg.Masse(dat.getMaterial(), volumen);
                txb_masse.Text = Convert.ToString("~" + (Math.Round(masse, round)) + " g");
            }

            //Fehler: Falsch Werte
            else
            {
                if (dat.getZähnezahl() % 1 != 0)
                {
                    MessageBox.Show("Bitte eine ganzzahlige Zähnezahl über 2 eingeben!");
                    txb_zaehnezahl_außen.Background = Brushes.OrangeRed;
                }
                if (dat.getZähnezahl() < 2)
                {
                    MessageBox.Show("Bitte eine ganzzahlige Zähnezahl über 2 eingeben!");
                    txb_zaehnezahl_außen.Background = Brushes.OrangeRed;
                }
                if (dat.getModul() <= 0)
                {
                    MessageBox.Show("Bitte Modul über 0 wählen!");
                    txb_modul_außen.Background = Brushes.OrangeRed;
                }
                if (dat.getBreite() <= 0)
                {
                    MessageBox.Show("Bitte Breite über 0 wählen!");
                    txb_breite_außen.Background = Brushes.OrangeRed;
                }
                if (dat.getSchrägungswinkel() <= 0)
                {
                    MessageBox.Show("Bitte Schrägungswinkel größer 0° und kleiner 90° wählen!");
                    txb_schraegungswinkel.Background = Brushes.OrangeRed;
                }
                if (dat.getSchrägungswinkel() >= 1.5704)
                {
                    MessageBox.Show("Bitte Schrägungswinkel größer 0° und kleiner 90° wählen!");
                    txb_schraegungswinkel.Background = Brushes.OrangeRed;
                }
            }
            return dat;
        }

        private Data BerechnungenGeradeInnen(Data dat)
        {
            Berechnungen prg = new Berechnungen();

            //If-Abfragen Korrekte Eingaben
            if (dat.getZähnezahl() % 1 == 0 && dat.getZähnezahl() >= 2 && dat.getModul() > 0 && dat.getBreite() > 0)
            {
                txb_breite_innen.Background = Brushes.White;
                txb_zaehnezahl_innen.Background = Brushes.White;
                txb_modul_innen.Background = Brushes.White;
                int round = Convert.ToInt32(cmb_rundung_innen.Text);

                double d = prg.Teilkreisdurchmesser_d(dat.getModul(), dat.getZähnezahl());
                dat.setTeilkreisdurchmesser(d);
                txb_teilkreisdurchmesser1.Text = Convert.ToString(Math.Round(d, round) + " mm");

                double teilung = prg.Teilung_p(dat.getKreiszahl(), dat.getModul());
                txb_teilung1.Text = Convert.ToString(Math.Round(teilung, round) + " mm");

                double kopfspiel = prg.Kopfspiel_c(dat.getModul(), dat.getKopfspielzahl());
                txb_kopfspiel1.Text = Convert.ToString(Math.Round(kopfspiel, round) + " mm");

                double innenKopfkreisdurchmesser = prg.Kopfkreisdurchmesser_dai(d, dat.getModul());
                txb_kopfkreisdurchmesser1.Text = Convert.ToString(Math.Round(innenKopfkreisdurchmesser, round) + "mm");

                double innenFußkreisdurchmesser = prg.Fußkreisdurchmesser_dfi(d, dat.getModul(), kopfspiel);
                txb_fußkreisdurchmesser1.Text = Convert.ToString(Math.Round(innenFußkreisdurchmesser, round) + "mm");

                double zahnhöhe = prg.Zahnhöhe_h(dat.getModul(), kopfspiel);
                txb_zahnhoehe1.Text = Convert.ToString(Math.Round(zahnhöhe, round) + " mm");

                double zahnkopfhöhe = prg.Zahnkopfhöhe_ha(dat.getModul());
                txb_zahnkopfhoehe1.Text = Convert.ToString(Math.Round(zahnkopfhöhe, round) + " mm");

                double zahnfüßhöhe = prg.Zahnfußhöhe_hf(dat.getModul(), kopfspiel);
                txb_zahnfußhoehe1.Text = Convert.ToString(Math.Round(zahnfüßhöhe, round) + " mm");
            }

            // Fehler: Falsche Werte
            else
            {
                if (dat.getZähnezahl() % 1 != 0 || dat.getZähnezahl() < 2)
                {
                    MessageBox.Show("Bitte eine ganzzahlige Zähnezahl über 2 eingeben!");
                    txb_zaehnezahl_innen.Background = Brushes.OrangeRed;
                }
                if (dat.getModul() <= 0)
                {
                    MessageBox.Show("Bitte Modul über 0 wählen!");
                    txb_modul_innen.Background = Brushes.OrangeRed;
                }
                if (dat.getBreite() <= 0)
                {
                    MessageBox.Show("Bitte Breite über 0 wählen!");
                    txb_breite_innen.Background = Brushes.OrangeRed;
                }
            }
            return dat;
        }



        //Programm beenden
        private void btn_beenden_ClickAußen(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Programm beenden?", "ZZ Zahnradrechner", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Application.Current.Shutdown();
                    break;
            }
        }

        private void btn_beenden_ClickInnen(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Programm beenden?", "ZZ Zahnradrechner", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Application.Current.Shutdown();
                    break;
            }
        }


        //Material
        public double material;
        public void cmb_materialwahl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmb_materialwahl.SelectedIndex == 0)
            {
                const double dichteVergütungsstahl = 7.84;      //C35/C45
                material = dichteVergütungsstahl;
            }
            if (cmb_materialwahl.SelectedIndex == 1)
            {
                const double dichteNichtrostenderStahl = 7.0;   //X12CrNiS188
                material = dichteNichtrostenderStahl;
            }
            if (cmb_materialwahl.SelectedIndex == 2)
            {
                const double dichteKunststoff = 1.41;           //POM
                material = dichteKunststoff;
            }
            if (cmb_materialwahl.SelectedIndex == 3)
            {
                const double dichteGusseisen = 7.2;             //GG
                material = dichteGusseisen;
            }
            if (cmb_materialwahl.SelectedIndex == 4)
            {
                const double dichteMessing = 8.5;               //CuZn
                material = dichteMessing;
            }
        }


        //ResetButtons
        private void btn_neu_Click(object sender, RoutedEventArgs e)
        {
            txb_modul_außen.Text = "";
            txb_breite_außen.Text = "";
            txb_zaehnezahl_außen.Text = "";
            txb_schraegungswinkel.Text = "";
            txb_teilkreisdurchmesser.Text = "";
            txb_teilung.Text = "";
            txb_kopfspiel.Text = "";
            txb_kopfkreisdurchmesser.Text = "";
            txb_fußkreisdurchmesser.Text = "";
            txb_zahnhoehe.Text = "";
            txb_zahnkopfhoehe.Text = "";
            txb_zahnfußhoehe.Text = "";
            txb_volumen.Text = "";
            txb_masse.Text = "";
            txb_grundkreisdurchmesser.Text = "";
            txb_stirnmodul.Text = "";
            txb_stirnteilung.Text = "";

            txb_modul_außen.Background = Brushes.White;
            txb_breite_außen.Background = Brushes.White;
            txb_schraegungswinkel.Background = Brushes.White;
            txb_zaehnezahl_außen.Background = Brushes.White;
        }

        private void botton_neu_Click(object sender, RoutedEventArgs e)
        {
            txb_modul_innen.Text = "";
            txb_zaehnezahl_innen.Text = "";
            txb_breite_innen.Text = "";
            txb_teilkreisdurchmesser1.Text = "";
            txb_teilung1.Text = "";
            txb_kopfspiel1.Text = "";
            txb_kopfkreisdurchmesser1.Text = "";
            txb_fußkreisdurchmesser1.Text = "";
            txb_zahnhoehe1.Text = "";
            txb_zahnkopfhoehe1.Text = "";
            txb_zahnfußhoehe1.Text = "";

            txb_modul_innen.Background = Brushes.White;
            txb_zaehnezahl_innen.Background = Brushes.White;
            txb_breite_innen.Background = Brushes.White;


        }


        //Info + Hinweis Buttons
        private void Btn_ClickInfo(object sender, RoutedEventArgs e)
        {
            Window1 Infowindow1 = new Window1();
            Infowindow1.Show();
        }

        private void Btn_ClickHinweise(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Der Kopfspielfaktor ist mit 0,167 belegt. " + Environment.NewLine + "Der Normeingriffswinkel beträgt 20°.");
        }

        private void Btn_ClickHinweiseInnen(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Der Kopfspielfaktor ist mit 0,167 belegt.");
        }


        //Ausblendung Schrägungswinkel
        private void rdbtn_gerade_Checked(object sender, RoutedEventArgs e)
        {
            lbl_schraegungswinkel.Visibility = Visibility.Hidden;
            txb_schraegungswinkel.Visibility = Visibility.Hidden;
        }

        private void rdbtn_gerade_Unchecked(object sender, RoutedEventArgs e)
        {
            lbl_schraegungswinkel.Visibility = Visibility.Visible;
            txb_schraegungswinkel.Visibility = Visibility.Visible;
        }

        private void rdbtn_schräg_Checked(object sender, RoutedEventArgs e)
        {
            lbl_schraegungswinkel.Visibility = Visibility.Visible;
            txb_schraegungswinkel.Visibility = Visibility.Visible;
        }


        //API
        private void btn_CatiaStart(object sender, RoutedEventArgs e)
        {
            Process P = new Process();
            P.StartInfo.FileName = "CNEXT.exe";
            P.Start();
        }

        public void btn_CatiaClick(object sender, RoutedEventArgs e)
        {
            Data dat = new Data();
        }      

<<<<<<< Updated upstream
        public void CatiaControl()
        {
            double d = 200;
            double m = 2;
            double z = 100;
            double b = 30;
=======
            //try
             //{
            CatiaConnection cc = new CatiaConnection();
>>>>>>> Stashed changes

            try
            {
                CatiaConnection cc = new CatiaConnection();

                // Finde Catia Prozess
                if (cc.CATIALaeuft())
                {
                    // Öffne ein neues Part
                    cc.ErzeugePart();

                    // Erstelle eine Skizze
                    cc.ErstelleLeereSkizze();

                    // Generiere ein Profil
                    cc.ErzeugeProfil(dat);

                    // Extrudiere Balken
                    cc.ErzeugeDasNeueKreismuster(dat);
                }
                else
                {
                    MessageBox.Show("Laufende Catia Application nicht gefunden");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception aufgetreten");
            }
        }
    }   
}
