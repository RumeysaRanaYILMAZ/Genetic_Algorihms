using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deneme_8
{

    class rastgelsin
    {
        //Kısa zamanda rastgele sayılar üretmeyi sağlar.
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public int Next(int minimum, int maximum)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(minimum, maximum);
            }
        }
    }
    class durakcı
    {
        //Durak sınıfı arac tipinde minibüs ve taksi alır. Aldığı araçların içindeki yolcuları inecekleri yolcular yönünden kontrol eder.
        public way onceki;
        public way sıradaki;
        public Bina baslangıc_binası;
        public List<passenger> durak_list;
        public List<tasitlar> araclar;
        int numara;
        public durakcı(int sayı)
        {
            this.numara = sayı;
            durak_list = new List<passenger>();
            araclar = new List<tasitlar>();
        }
        public durakcı(int sayı, Bina bas)
        {
            baslangıc_binası = bas;
            this.numara = sayı;
            durak_list = new List<passenger>();
            araclar = new List<tasitlar>();
        }
        public void tasit_kontrol()
        {
            //tasıttaki yolcuların ineceği durağın bu olup olmadığını kontrol eder.Bu durakta ineceklerse tasıtların yolcu listesinde o yolcuyu kendi yolcu listesine alır.
            if (isEmpty() == false)
            {
                foreach (tasitlar araba in araclar)
                {
                    if (araba.icerdekiler.Count != 0)
                    {
                        List<passenger> tempera = new List<passenger>();
                        foreach (passenger a in araba.icerdekiler)
                        {
                            if (a.durak == numara)
                            {
                                passenger temp = a;
                                a.its = araba.süre;

                                durak_list.Add(temp);
                            }
                            else
                            {
                                tempera.Add(a);
                            }
                        }
                        araba.icerdekiler = tempera;
                    }
                    if (araba.kapasite == 4)
                    {
                        if (araba.icerdekiler.Count != 0)
                        {
                            sıradaki.yoldakiler.Add(araba);
                        }
                        else
                        {
                            araba.donus = true;
                            onceki.yoldakiler.Add(araba);
                        }
                    }
                    else
                    {
                        if (this.numara == 40 && araba.donus == false)
                        {
                            araba.donus = true;
                            onceki.yoldakiler.Add(araba);
                        }
                        else if (araba.donus == true)
                        {
                            onceki.yoldakiler.Add(araba);
                        }
                        else
                        {
                            sıradaki.yoldakiler.Add(araba);
                        }

                    }
                }
                araclar.Clear();
            }
        }
        //Durağın dolu olup olmadığını kontrol eden fonksiyon
        public bool isEmpty()
        {
            if (araclar.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class Bina : durakcı
    {
        public List<passenger> taksi_list = new List<passenger>();
        public List<passenger> mini_list = new List<passenger>();
        public List<tasitlar> tum_arac;
        public Bina() : base(0)
        {
        }

        public List<tasitlar> baslat()
        {
            //Ana işlemlerin başlaması ve tasit tiplerinin istenen sayıda üretilmesini sağlar.
            List<tasitlar> baslangicAraclari = new List<tasitlar>();
            baslangicAraclari.Add(new Minibus());
            baslangicAraclari.Add(new Taksi());
            araclar = baslangicAraclari;
            List<tasitlar> tum_araclar = new List<tasitlar>();
            foreach (tasitlar t in baslangicAraclari)
            {
                tum_araclar.Add(t);
            }
            tum_arac = tum_araclar;
            dagıt();
            return tum_araclar;
        }
        public void dagıt()
        {
            //Eğer binanın içinde tasit varsa ve binada yolcu da varsa bekleyen yolcuları ilgili tasıtın özelliklerine göre araçlara dağıtır.
            //Eğer tasıtların süresi 600 saniyeye bölünüyorsa bu 10 dakika geçtiği anlamına gelir ve yeni bir taksi ve yeni bir minibüs üretir.
            tasitlar t = tum_arac.First();
            if (t.süre % 600 == 0 && t.süre != 0 && taksi_list.Count != 0 && mini_list.Count != 0)
            {
                Minibus yeni1 = new Minibus();
                yeni1.süre = t.süre;
                Taksi yeni2 = new Taksi();
                yeni2.süre = t.süre;
                araclar.Add(yeni1);
                araclar.Add(yeni2);
                tum_arac.Add(yeni1);
                tum_arac.Add(yeni2);
            }
            while (araclar.Count != 0)
            {
                tasitlar arac = araclar.First();
                arac.donus = false;
                if (arac.kapasite == 4)
                {
                    arac.binme(taksi_list);
                    araclar.Remove(arac);
                    sıradaki.yoldakiler.Add(arac);
                }
                else
                {
                    arac.binme(mini_list);
                    araclar.Remove(arac);
                    sıradaki.yoldakiler.Add(arac);
                }
            }
        }
    }
    class way
    {
        public durakcı prev;
        public durakcı next;
        public List<tasitlar> yoldakiler;
        public way()
        {
            yoldakiler = new List<tasitlar>();
        }
        public void ilerle()
        {
            //yoldaki araçların sürelerinin ilerlemeye müsait olup olmadığını kontrol eder ve müsait ise tasitin dönüs değişkeninin değerine göre ileri ve geri gönderir.
            if (isEmpty() == false)
            {
                while (yoldakiler.Count != 0)
                {
                    List<tasitlar> temper = new List<tasitlar>();
                    foreach (tasitlar arac in yoldakiler)
                    {
                        arac.süre += 75;
                        if (arac.kapasite == 4 && arac.süre % 225 == 0 && arac.donus == false)
                        {
                            tasitlar temp1 = arac;
                            next.araclar.Add(temp1);
                        }
                        else if (arac.kapasite == 4 && arac.süre % 225 == 0 && arac.donus == true)
                        {
                            tasitlar temp1 = arac;
                            prev.araclar.Add(temp1);
                        }
                        else if (arac.kapasite == 6 && arac.süre % 300 == 0 && arac.donus == false)
                        {
                            tasitlar temp2 = arac;
                            next.araclar.Add(temp2);
                        }
                        else if (arac.kapasite == 6 && arac.süre % 300 == 0 && arac.donus == true)
                        {
                            tasitlar temp2 = arac;
                            prev.araclar.Add(temp2);
                        }
                        else
                        {
                            temper.Add(arac);
                        }
                    }
                    yoldakiler = temper;
                }
            }
        }
        public bool isEmpty()
        {
            if (yoldakiler.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class passenger : IComparable
    {
        public tasitlar t = null;
        public int atama;
        public int durak;
        public int its;
        public string name;
        public passenger(string isim, int durmayak, int say)
        {
            this.name = isim;
            this.durak = durmayak;
            this.atama = say;
        }
        public string tostring()
        {
            string temp =string.Format( "ITS: {0:N2}",(double)( its / 60)) + "   İsim:" + name + "   Durak:" + durak;
            if (t.kapasite == 4)
            {
                temp += "    Taksi";
            }
            else
            {
                temp += "    Minibüs";
            }
            return temp;
        }
        //yolcuların oitslerinin karşılaştırılabilmesine imkan veren bir fonksiyondur.
        public int CompareTo(object obj)
        {
            passenger diger = (passenger)obj;
            if (this.durak > diger.durak)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
    abstract class tasitlar
    {
        public bool donus;
        public List<passenger> icerdekiler;
        public int süre;
        public int kapasite;
        public tasitlar(int capacity)
        {
            this.kapasite = capacity;
            icerdekiler = new List<passenger>();
        }
        public abstract void binme(List<passenger> bekleyen);
        public abstract string toString();
    }
    class Taksi : tasitlar
    {
        public Taksi() : base(4)
        {
            donus = false;
        }
        //Kendi biniş özelliklerine göre verilen yolcu listesinden yolcuları alıp kendi listesine atar.
        public override void binme(List<passenger> bekleyen)
        {
            List<passenger> temp = new List<passenger>();
            foreach (passenger y in bekleyen)
            {
                temp.Add(y);
            }
            temp.Sort();
            int a = 0;
            while (a < 4)
            {
                if (temp.Count == 0)
                {
                    break;
                }
                passenger yeni = temp.First();
                temp.Remove(yeni);
                yeni.t = this;
                bekleyen.Remove(yeni);
                icerdekiler.Add(yeni);
                a++;
            }
        }

        public override string toString()
        {
            string tak = "Taksi   ITS:" + this.süre;
            return tak;
        }
    }
    class Minibus : tasitlar
    {

        public Minibus() : base(6)
        {
            donus = false;
        }
        //Kendi biniş özelliklerine göre verilen yolcu listesinden yolcuları alıp kendi listesine atar.
        public override void binme(List<passenger> bekleyen)
        {
            int y = 0;
            while (y < 6)
            {
                if (bekleyen.Count == 0)
                {
                    break;
                }
                passenger yeni = bekleyen.First();
                yeni.t = this;
                bekleyen.Remove(yeni);
                icerdekiler.Add(yeni);
                y++;
            }
        }

        public override string toString()
        {
            string min = "Minibüs   ITS:" + this.süre;
            return min;
        }
    }
    class mesafe
    {
        //Durak ve yollların yapısallığının sağlanabilmesi için oluşturulmuş bir class'tır.
        public way yol1;
        public durakcı durak1;
        public mesafe(int sıra)
        {
            yol1 = new way();
            durak1 = new durakcı(sıra);
            yol1.next = durak1;
            durak1.onceki = yol1;
        }
    }
    class Program1
    {

        string[] isimler = new string[] { "Selda", "Aslı", "Uras", "Deniz", "Arda", "Büşra"
, "Burak", "Sinem", "Arun", "Boran", "Nisa", "Daniel", "Keke", "Silay", "Harun", "Liya", "Biran"
, "Seren", "Enez", "Esin", "Ahri", "Nida", "Nazır", "Hatice", "Seçkin" };
        int[] dizi = new int[] { 16, 23, 39, 17, 12, 10, 11, 2, 23, 18, 35, 36, 4, 31, 25, 27, 6, 13, 1, 36, 15, 7, 18, 34, 10 };
        List<passenger> yolcuxlist = new List<passenger>();
        mesafe[] guzergah = new mesafe[40];
        Bina is_yeri = new Bina();
        bool finish_arac(List<tasitlar> araclar)
        {
            //Araçları ve binadaki yolcu var mı kotrol eder.
            bool bos = true;
            foreach (tasitlar t in araclar)
            {
                if (t.icerdekiler.Count != 0)
                {
                    bos = false;
                    break;
                }
            }
            return bos;
        }
        bool finish(List<tasitlar> araclar, Bina is_yeri)
        {
            // programın bitiş fonksiyonunun şartını kontrol eder.
            if (is_yeri.taksi_list.Count == 0 && is_yeri.mini_list.Count == 0 && finish_arac(araclar) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<tasitlar> duzenleme(int s, int[] atama)
        {
            //İlerleme yapısını oluşturur.
            int r = 0;
            while (r < s)
            {
                guzergah[r] = new mesafe(r + 1);
                guzergah[r].durak1.baslangıc_binası = is_yeri;
                r++;
            }
            for (int i = 1; i < s; i++)
            {
                guzergah[i - 1].durak1.sıradaki = guzergah[i].yol1;
                guzergah[i].yol1.prev = guzergah[i - 1].durak1;
            }
            is_yeri.sıradaki = guzergah[0].yol1;
            guzergah[0].yol1.prev = is_yeri;
            for (int i = 0; i < 25; i++)
            {
                passenger yolunda_gerek = new passenger(isimler[i], dizi[i], atama[i]);
                yolcuxlist.Add(yolunda_gerek);
            }
            foreach (passenger y in yolcuxlist)
            {
                if (y.atama == 0)
                {
                    is_yeri.taksi_list.Add(y);
                }
                else
                {
                    is_yeri.mini_list.Add(y);
                }
            }
            List<tasitlar> tum_araclar = is_yeri.baslat();
            return tum_araclar;
        }
        public void tostring()
        {
            foreach (passenger i in yolcuxlist)
            {
                Console.WriteLine(i.tostring());
            }
        }
        public int Gerkeclestirme(int[] atama)
        {
            //İlerleme işlemini yapar.
            List<tasitlar> tum_araclar = duzenleme(40, atama);
            is_yeri.dagıt();
            is_yeri.tum_arac = tum_araclar;
            while (finish(is_yeri.tum_arac, is_yeri) == false)
            {
                foreach (mesafe m in guzergah)
                {
                    m.yol1.ilerle();
                }
                foreach (mesafe m in guzergah)
                {
                    m.durak1.tasit_kontrol();
                }
                is_yeri.dagıt();
            }
            int p = 0;
            foreach (passenger y in yolcuxlist)
            {
                p += y.its;
            }
            return p / yolcuxlist.Count;
        }

    }

    class genom : IComparable
    {
        //Genetik sınıfının çalışma elemanlarından biri
        public int[] binis = new int[25];
        public int oits;
        rastgelsin rand = new rastgelsin();
        public int CompareTo(object obj)
        {
            genom g = (genom)obj;
            if (g.oits < this.oits)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public void random()
        {
            for (int y = 0; y < 25; y++)
            {
                binis[y] = rand.Next(0, 2);
            }
        }
    }

    class genetic
    {
        public rastgelsin rand = new rastgelsin();

        public List<genom> random(int tekrar)
        {
            //Random biniş kararları oluşturur.
            List<genom> ornek_uzay = new List<genom>();
            for (int i = 0; i < tekrar; i++)
            {
                genom a = new genom();
                a.random();
                ornek_uzay.Add(a);
            }
            return ornek_uzay;
        }

        public List<genom> isleme(List<genom> arac)
        {
            //Oluşturulan kararların işlenmesi ve oitslerin üretilmesi
            List<genom> elit = new List<genom>();
            foreach (genom gen in arac)
            {
                Program1 y = new Program1();
                gen.oits = y.Gerkeclestirme(gen.binis);
            }
            arac.Sort();
            int p = 0;
            while (p < 10)
            {
                genom gnn = arac.First();
                elit.Add(gnn);
                arac.Remove(gnn);
                p++;
            }
            arac = null;
            return elit;
        }
        static genom Copy(int[] goal)
        {
            genom g1 = new genom();
            int[] cop = new int[25];
            for (int g = 0; g < 25; g++)
            {
                cop[g] = goal[g];
            }
            g1.binis = cop;
            return g1;
        }
        genom[] karısım(genom g, genom n)
        {// Crossing over işleminin alt yapısını sağlar.
            int r1 = rand.Next(0, 22);
            genom g1 = Copy(g.binis);
            genom g2 = Copy(n.binis);
            for (int d = r1; d < (r1 + 3); d++)
            {
                g1.binis[d] = n.binis[d];
                g2.binis[d] = g.binis[d];
            }
            genom[] cros = { g1, g2 };
            return cros;
        }
        public List<genom> cross_over(List<genom> ata)
        {
            //Elemanların küçük parçalarının değiştirilmesi işlemi
            genom[] dol = ata.ToArray();
            for (int i = 0; i < 45; i++)
            {
                int f = rand.Next(0, 10);
                int s = rand.Next(0, 10);
                while (s == f)
                {
                    f = rand.Next(0, 10);
                    s = rand.Next(0, 10);
                }
                genom[] a = karısım(dol[f], dol[s]);
                ata.Add(a[1]);
                ata.Add(a[0]);
            }
            return ata;
        }
        public genom dongu(int tekrar)
        {
            int t = 0;
            List<genom> ara_list = cross_over(isleme(random(50)));
            while (t < tekrar)
            {
                ara_list = cross_over(isleme(ara_list));
                t++;

            }
            return ara_list.First();
        }


    }

    //sometimess thngs just take
    class Randomm
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public int Next(int minimum, int maximum)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(minimum, maximum);
            }
        }
    }
    class chromosome
    {
        public double oits;
        public double fitness_value;
        public int cross_count;
        public int[] dizi = new int[25];
        public chromosome()
        {
            oits = 0;
            fitness_value = 0;
            cross_count = 0;
        }
        public chromosome(int[] dizi, List<yolcu> yolculist)//kromozom oluşturulur oitsler hesaplanır.
        {
            this.dizi = dizi;
            List<yolcu> tlist = new List<yolcu>();
            List<yolcu> mlist = new List<yolcu>();
            for (int i = 0; i < dizi.Length; i++)//oits hesaplamak için 0 ları taksi listesine 1 leri minibüs listesine atar.
            {
                if (dizi[i] == 0)
                {
                    yolcu yolcu = (yolcu)yolculist[i]; //dizinin elemanının o anki değeri 0 ise , o indeksteki eleman taksiye binmelidir.
                    tlist.Add(yolcu);

                }
                else
                {
                    yolcu yolcu = (yolcu)yolculist[i];//dizinin elemanının o anki değeri 1 ise , o indeksteki eleman minibüse binmelidir.
                    mlist.Add(yolcu);
                }
            }

            //Bu kısım taksidekiler için itsleri toplar
            int tsay = tlist.Count();
            double oits = 0;
            double bekleme_süresi = 0;
            int max = 0;
            int y = 0, f = 0;
            while (tsay != 0)
            {

                for (int i = 0; i < 4; i++)//4 yolcuda süre hesaplar her 4 yolcuda bekleme süresi eşittir.
                {
                    if (y < tlist.Count)
                    {
                        yolcu temp = (yolcu)tlist[y];
                        double p = bekleme_süresi + (temp.durak * 3.75);
                        temp.its3 = p;
                        oits += p;
                        tsay--;
                        // Console.WriteLine(temp.ToString() + " ITS =  " + p);

                    }
                    y++;

                }

                for (int i = 0; i < 4; i++)//hesaplama için bölümün max numaralı durakını alır bekleme süresini hesaplatır.
                {
                    if (f < tlist.Count)
                    {
                        yolcu tempx = (yolcu)tlist[f];

                        if (tempx.durak > max)
                        {
                            max = tempx.durak;
                        }
                    }
                    f++;

                }

                bekleme_süresi += max * 3.75;
                max = 0;

            }
            //Bu kısım minibüstekiler için itsleri toplar
            double bekleme_süresix = 0;
            int maxx = 0;
            int yy = 0, ff = 0;
            tsay = mlist.Count();

            while (tsay != 0)//minibüs itsleri yazdırır.
            {


                for (int i = 0; i < 6; i++)
                {
                    if (yy < mlist.Count)
                    {
                        yolcu temp = (yolcu)mlist[yy];
                        double p = bekleme_süresix + (temp.durak * 5);
                        temp.its3 = p;
                        oits += p;
                        tsay--;

                        // Console.WriteLine(temp.ToString() + " ITS =  " + p);
                    }
                    yy++;

                }

                for (int i = 0; i < 6; i++)
                {
                    if (ff < mlist.Count)
                    {
                        yolcu tempx = (yolcu)mlist[ff];

                        if (tempx.durak > maxx)
                        {
                            maxx = tempx.durak;
                        }
                    }
                    ff++;

                }


                bekleme_süresix += maxx * 5;
                maxx = 0;
            }
            oits = oits / 25;
            this.oits = oits;



        }
        public override string ToString()
        {
            return "fitness value =" + fitness_value + "      oits = " + oits;
        }
    }





    /// <summary>
    /// //////// Non-Genetic
    /// </summary>
    class yolcu
    {
        public int durak;
        public double its1,its2,its3;
        public string name;
        public yolcu(string isim, int durmayak)
        {
            this.name = isim;
            this.durak = durmayak;
        }
        public yolcu()
        {
            name = "yok";
        }
        public override string ToString()
        {
            return this.name + " adlı kişinin ineceği durak " + this.durak + ". duraktır.";
        }
    }
    class tasit
    {
        public List<yolcu> icerdekiler;
        public List<yolcu> asilList;
        public double süre;
        public int kapasite, gidis_say;
        int max = -1, min = 90000;


        public tasit(int capacity)
        {
            icerdekiler = new List<yolcu>();
            asilList = new List<yolcu>();
            this.kapasite = capacity;
            süre = 0;
        }
        public bool arac_dolu_mu()
        {
            if (icerdekiler.Count >= kapasite)
            {
                return true;
            }
            return false;

        }

        public void icerdekilere_ekle(yolcu yolcu)
        {
            if (this.kapasite == 4)//taksiye pq atama yapar.
            {
                this.icerdekiler.Add(yolcu);

                if (yolcu.durak >= max)
                {

                    this.asilList.Insert((asilList.Count), yolcu);
                    max = yolcu.durak;

                }
                else if (yolcu.durak <= min)
                {

                    asilList.Insert(0, yolcu);
                    min = yolcu.durak;

                }
                else
                {
                    for (int s = 0; s < asilList.Count; s++)
                    {
                        yolcu yolcux = new yolcu();
                        yolcux = (yolcu)asilList[s];
                        yolcu yolcux2 = new yolcu();
                        yolcux2 = (yolcu)asilList[s + 1];

                        if (yolcu.durak >= yolcux.durak && yolcu.durak <= yolcux2.durak)
                        {
                            asilList.Insert(s + 1, yolcu);
                            break;

                        }
                    }

                }


            }
            else//minibüse fifo atama
            {
                this.icerdekiler.Add(yolcu);
                this.asilList.Add(yolcu);
            }

        }
    }
    class TaksiPQ : tasit
    {
        public TaksiPQ() : base(4)
        {
        }
    }
    class MinibusFIFO : tasit
    {
        public MinibusFIFO() : base(6)
        {
        }
    }
    class minoits
    {
        public double minsüre;
        public ArrayList liste;
        public minoits()
        {
            minsüre = 80000;
            liste = new ArrayList();
        }
    }

    class Program
    {

        public List<chromosome> ComputeFitness(List<chromosome> chromosomeList, List<yolcu> yolcu_list)
        {  
            //oluşturulan kromozomların uygunluk değerlerini hesaplaması için  roulettwheel ile entegre çalışır.
            double toplam_oits = 0;
            if (chromosomeList.Count > 30)
            {
                List<chromosome> chromosomeList2 = new List<chromosome>();

                for (int i = 30; i < 60; i++)
                {
                    toplam_oits += chromosomeList[i].oits;
                    chromosome kromozom = new chromosome(chromosomeList[i].dizi, yolcu_list);


                    chromosomeList2.Add(kromozom);


                }
                for (int i = 0; i < 30; i++)
                {

                    chromosomeList.RemoveAt(chromosomeList.Count - 1);

                }
                double toplam_oitss = 0;

                chromosomeList2 = Roulette_wheel(chromosomeList2, toplam_oits);
                foreach (chromosome kromozom in chromosomeList2)
                {
                    chromosomeList.Add(kromozom);
                }
                chromosomeList = Roulette_wheel2(chromosomeList);


            }
            else
            {
                foreach (chromosome kromozom in chromosomeList)
                {
                    toplam_oits += kromozom.oits;
                }
                chromosomeList = Roulette_wheel(chromosomeList, toplam_oits);
            }

            while (chromosomeList.Count > 30)//en verimsizleri siler
            {
                chromosomeList.RemoveAt(chromosomeList.Count - 1);
            }
            return chromosomeList;



        }

        public List<chromosome> Roulette_wheel(List<chromosome> chromosomeList, double toplam_oits)//büyük listenin sonunda küçük en başta
        {
            List<chromosome> chromosomeList2 = new List<chromosome>();
            double max = -1, min = 90000;
            foreach (chromosome kromozom in chromosomeList)//büyük listenin sonunda küçük en başta 
            {
                double fv = kromozom.oits / toplam_oits;
                kromozom.fitness_value = fv;
                if (fv >= max)
                {


                    chromosomeList2.Insert((chromosomeList2.Count), kromozom);
                    max = fv;

                }
                else if (fv <= min)
                {

                    chromosomeList2.Insert(0, kromozom);
                    min = fv;

                }
                else
                {
                    for (int s = 0; s < chromosomeList2.Count; s++)
                    {
                        chromosome yolcux = new chromosome();
                        yolcux = (chromosome)chromosomeList2[s];
                        chromosome yolcux2 = new chromosome();
                        yolcux2 = (chromosome)chromosomeList2[s + 1];

                        if (fv >= yolcux.fitness_value && fv <= yolcux2.fitness_value)
                        {
                            chromosomeList2.Insert(s + 1, kromozom);
                            break;

                        }
                    }

                }
            }


            return chromosomeList2;
        }
        public List<chromosome> Roulette_wheel2(List<chromosome> chromosomeList)//oitsi büyük olan kromozom listenin sonunda küçük en başta
        {
            List<chromosome> chromosomeList2 = new List<chromosome>();
            double max = -1, min = 90000;
            foreach (chromosome kromozom in chromosomeList)//büyük listenin sonunda küçük en başta 
            {
                double fv = kromozom.fitness_value;

                if (fv >= max)
                {


                    chromosomeList2.Insert((chromosomeList2.Count), kromozom);
                    max = fv;

                }
                else if (fv <= min)
                {

                    chromosomeList2.Insert(0, kromozom);
                    min = fv;

                }
                else
                {
                    for (int s = 0; s < chromosomeList2.Count; s++)
                    {
                        chromosome yolcux = new chromosome();
                        yolcux = (chromosome)chromosomeList2[s];
                        chromosome yolcux2 = new chromosome();
                        yolcux2 = (chromosome)chromosomeList2[s + 1];

                        if (fv >= yolcux.fitness_value && fv <= yolcux2.fitness_value)
                        {
                            chromosomeList2.Insert(s + 1, kromozom);
                            break;

                        }
                    }

                }
            }


            return chromosomeList2;
        }
        public List<chromosome> Crossover(List<chromosome> chromosomeList, Randomm rnd, List<yolcu> yolcu_list)
        {
            //kromozomlarda çaprazlama yapar.
            for (int j = 0; j < 29; j += 2)
            {
                int bölüt_Noktası = rnd.Next(0, 18);
                chromosome kromozom1;
                chromosome kromozom2;
                if (j < 10)//ilk 7 kromozom önceliklidir.
                {
                    int xx = rnd.Next(0, 7);
                    kromozom1 = (chromosome)chromosomeList[xx];
                    kromozom2 = (chromosome)chromosomeList[j];

                }

                else
                {
                    kromozom1 = (chromosome)chromosomeList[j];
                    kromozom2 = (chromosome)chromosomeList[j + 1];
                }

                int[] yeni_dizi = new int[25];
                int[] yeni_dizi2 = new int[25];
                for (int i = 0; i < 25; i++)
                {



                    if (i < bölüt_Noktası)
                    {
                        yeni_dizi[i] = kromozom1.dizi[i];
                        yeni_dizi2[i] = kromozom2.dizi[i];

                    }
                    else
                    {

                        {
                            yeni_dizi[i] = chromosomeList[j + 1].dizi[i];
                            yeni_dizi2[i] = chromosomeList[j].dizi[i];
                        }
                    }


                }
                chromosome kromozom3 = new chromosome(yeni_dizi, yolcu_list);
                chromosome kromozom4 = new chromosome(yeni_dizi2, yolcu_list);

                chromosomeList.Add(kromozom3);
                chromosomeList.Add(kromozom4);
            }
            return chromosomeList;
        }//çaprazlama yapar
        public List<chromosome> Mutation(List<chromosome> chromosomeList, Randomm rnd, List<yolcu> yolcu_list)
        {
            //kromozomları mutasyona tabi tutar.
            for (int i = 30; i < 60; i++)
            {
                
                    int a = rnd.Next(0, 25);
                    int b = chromosomeList[i].dizi[a];
                    if (b == 0)
                    {
                        chromosomeList[i].dizi[a] = 1;

                        chromosome mutasyon = new chromosome(chromosomeList[i].dizi, yolcu_list);
                        chromosomeList.RemoveAt(i);
                        chromosomeList.Insert(i, mutasyon);

                    }
                    else
                    {
                        chromosomeList[i].dizi[a] = 0;
                        chromosome mutasyon = new chromosome(chromosomeList[i].dizi, yolcu_list);
                        chromosomeList.RemoveAt(i);
                        chromosomeList.Insert(i, mutasyon);


                    }
                
            }
            return chromosomeList;
        }
        public double Ondalikli(double sayi, int virguldenSonra = 4)
        {
            //girilen double sayıyı virgülden sonra 4 haneyle döndürür.
            string format = "0.";
            for (int i = 0; i < virguldenSonra; i++)
            {
                format += "#";
            }

            double yeniSayi = double.Parse(sayi.ToString(format));
            if (yeniSayi > sayi)
            {
                double fark = 1 / Math.Pow(10, virguldenSonra);
                yeniSayi -= fark;
            }
            return yeniSayi;

        }
        public bool esit_mi(List<ArrayList> listeninlistesi, ArrayList liste)
        {
            int eslik_say = 0;
            foreach (ArrayList taranan in listeninlistesi)
            {
                eslik_say = 0;
                for (int i = 0; i < taranan.Count; i++)
                {
                    int a = (int)taranan[i];
                    foreach (int c in liste)
                    {
                        if (a == c)
                        {
                            eslik_say++;

                        }
                    }
                }
                if (eslik_say >= 12)
                {
                    return true;
                }

            }


            return false;

        }//listenin listesinde listenin içeriğinin aynı olduğu bir liste varmı diye kontrol eder.
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("----------------------------------PROJE 2A 1.BÖLÜME HOŞGELDİNİZ----------------------------------");
            Random rnd = new Random();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            int program_say = 9;
            while (program_say != 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("ÇALIŞTIRMAK İSTEDİĞİNİZ PROJE BÖLÜMÜNÜ SEÇİNİZ, İSTEDİĞİNİZ PROJE NUMARASINI GİRİNİZ");
                
                Console.WriteLine("     1- ORTALAMA İŞLEM TAMAMLAMA SÜRELERİ");
                
                Console.WriteLine("     2- EN UYGUN TAŞIT BELİRLEME");
                
                Console.WriteLine("     3- 40 DURAK 25 KİŞİ İÇİN OİTS BELİRLEME");
                
                Console.WriteLine("     4- 40 DURAK 25 KİŞİ İÇİN EN UYGUN TAŞIT BELİRLEME (GENETİK ALGORİTMA İLE)");
                
                Console.WriteLine("     5- 40 DURAK 25 KİŞİ İÇİN 10 DAKİKADA BİR ARAÇ GÖNDERME (GENETİK ALGORİTMA İLE)");
                

                Console.WriteLine("Çıkmak için 0 giriniz.");


                program_say = Convert.ToInt32(Console.ReadLine());
                if (program_say == 0)
                {
                    Console.WriteLine("Hoşçakalın...");
                }
                if (program_say == 1)//ortalama işlem tamamlama sürelerini yazdırır.
                {
                    string[] isimler = new string[] { "Selda", "Aslı", "Uras", "Deniz", "Arda", "Büşra", "Burak", "Sinem", "Arun", "Boran", "Nisa", "Daniel", "Keke", "Silay", "Harun", "Liya", "Biran", "Doğukan", "İbrahim", "Seren", "Enez", "Esin", "Ahri", "Nida", "Nazır", "Hatice", "Seçkin" };
                    int[] dizi = new int[] { 6, 3, 9, 7, 12, 10, 11, 2, 3, 8, 5, 6, 4, 1, 5, 7, 6, 3 };
                    List<yolcu> yolcuxlist = new List<yolcu>();

                    for (int i = 0; i < 18; i++)
                    {
                        yolcu yolunda_gerek = new yolcu(isimler[i], dizi[i]);
                        yolcuxlist.Add(yolunda_gerek);

                    }



                    //a1.bölüm
                    int bölüm_say = 1;
                    if (bölüm_say == 1)
                    {
                        Console.WriteLine("------------------------Sadece Minibüs Çalıştığında------------------------");
                        MinibusFIFO minibüs = new MinibusFIFO();
                        int yolcu_say = yolcuxlist.Count;
                        while (!(yolcu_say == 0))
                        {
                            for (int i = 0; i < 6; i++)//taksiye gidiş sayısı bulduracak
                            {
                                if (yolcu_say != 0)
                                {
                                    yolcu_say--;
                                }

                            }
                            minibüs.gidis_say++;
                        }
                        minibüs.asilList = yolcuxlist;
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (minibüs.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 6; i++)
                            {
                                if (y < minibüs.asilList.Count)
                                {
                                    yolcu temp = (yolcu)minibüs.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 5);
                                    temp.its1 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 6; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < minibüs.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)minibüs.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 5;
                            max = 0;

                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        oits = oits / 18;
                        Console.WriteLine("i)Sadece minibüs çalıştığında oits = " + oits);
                        Console.WriteLine("          ");
                        Console.WriteLine("           ");


                    }

                    bölüm_say++;
                    //a2.bölüm
                    if (bölüm_say == 2)
                    {
                        Console.WriteLine("------------------------Sadece Taksi Çalıştığında------------------------");
                        TaksiPQ takzi = new TaksiPQ();
                        int yolcu_say = yolcuxlist.Count;
                        while (!(yolcu_say == 0))
                        {
                            for (int i = 0; i < 4; i++)//taksiye gidiş sayısı bulduracak
                            {
                                if (yolcu_say != 0)
                                {
                                    yolcu_say--;
                                }

                            }
                            takzi.gidis_say++;
                        }
                        foreach (yolcu a in yolcuxlist)
                        {
                            takzi.icerdekilere_ekle(a);
                        }
                        takzi.icerdekiler.Clear();
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (takzi.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 4; i++)
                            {
                                if (y < takzi.asilList.Count)
                                {
                                    yolcu temp = (yolcu)takzi.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 3.75);
                                    temp.its2 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 4; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < takzi.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)takzi.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 3.75;
                            max = 0;

                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        oits = oits / 18;
                        Console.WriteLine("i)Sadece taksi çalıştığında oits = " + oits);
                        Console.WriteLine("          ");
                        Console.WriteLine("           ");


                    }
                    bölüm_say++;


                    //a3.bölüm
                    if (bölüm_say == 3)
                    {
                        Console.WriteLine("------------------------ Taksi ve Minibüs Çalıştığında------------------------");
                        MinibusFIFO minibiz = new MinibusFIFO();
                        TaksiPQ takzi = new TaksiPQ();
                        List<yolcu> yolculist = new List<yolcu>();
                        foreach (yolcu yolc in yolcuxlist)
                        {
                            yolculist.Add(yolc);
                        }





                        while (yolculist.Count != 0)//aracın kaç kere yolcu alacağını belirleyen (gidişsay)
                        {

                            for (int i = 0; i < 2; i++)
                            {
                                if ((takzi.süre % 2250 == 0) && (minibiz.süre % 6000 == 0))//aynı anda durakta olma durumları random dağıtım yapılır.
                                {
                                    
                                    for (int j = 0; j <= yolculist.Count; j++)// minibüs ve taksi aynı anda işyerinden çalışanları almaya geldilerse işçiler araçlara random olarak biner.
                                    {

                                        if (yolculist.Count != 0)
                                        {
                                            yolcu yolcux = (yolcu)yolculist[0];
                                            int a = rnd.Next(0, 2);
                                            if (a == 0 && takzi.arac_dolu_mu() == false)
                                            {
                                                takzi.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }
                                            else if (a == 1 && minibiz.arac_dolu_mu() == false)
                                            {
                                                minibiz.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }
                                            else if (minibiz.arac_dolu_mu() == false)
                                            {
                                                minibiz.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }

                                            else if (takzi.arac_dolu_mu() == false)
                                            {

                                               
                                                takzi.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);

                                            }
                                            else
                                            {
                                                break;//Kimse kalmadı binada
                                            }

                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    takzi.icerdekiler.Clear();
                                    takzi.gidis_say++;
                                    minibiz.icerdekiler.Clear();
                                    minibiz.gidis_say++;
                                }
                                if (takzi.süre % 2250 == 0)//durakta o an sadece taksi geldiyse
                                {
                                    int m = -1;
                                    for (int j = 0; j < yolculist.Count; i++)
                                    {
                                        yolcu yolcux = (yolcu)yolculist[j];
                                        if (takzi.arac_dolu_mu() == false)
                                        {
                                            if (yolcux.durak > m)////////
                                            {
                                                m = yolcux.durak;
                                            }///
                                            takzi.icerdekilere_ekle(yolcux);//icerdekilere ekle fonksiyonu sayesinde  yolcuları asil listeye ekleriz.
                                            yolculist.Remove(yolcux);
                                        }
                                        else
                                        {
                                            takzi.süre += (12 - m) * 18.75;//////////////
                                            break;
                                        }

                                    }
                                    takzi.icerdekiler.Clear();
                                    takzi.gidis_say++;


                                }
                                if (minibiz.süre % 6000 == 0)//sadece minibüs ilk duraktayken
                                {
                                    for (int j = 0; j < yolculist.Count; i++)
                                    {
                                        yolcu yolcux = (yolcu)yolculist[j];
                                        if (minibiz.arac_dolu_mu() == false)
                                        {
                                            minibiz.icerdekilere_ekle(yolcux);
                                            yolculist.Remove(yolcux);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    minibiz.icerdekiler.Clear();
                                    minibiz.gidis_say++;

                                }
                                minibiz.süre += 15;////////////////
                                takzi.süre += 15;////////
                            }

                        }
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (takzi.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 4; i++)
                            {
                                if (y < takzi.asilList.Count)
                                {
                                    yolcu temp = (yolcu)takzi.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 3.75);
                                    temp.its3 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 4; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < takzi.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)takzi.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 3.75;
                            max = 0;
                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        Console.WriteLine("          ");
                        Console.WriteLine("           ");

                        double bekleme_süresix = 0;
                        int maxx = 0;
                        int yy = 0, ff = 0;

                        for (int j = 0; j < (minibiz.gidis_say); j++)//minibüs itsleri yazdırır.
                        {


                            for (int i = 0; i < 6; i++)
                            {
                                if (yy < minibiz.asilList.Count)
                                {
                                    yolcu temp = (yolcu)minibiz.asilList[yy];
                                    double p = bekleme_süresix + (temp.durak * 5);
                                    temp.its3 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);
                                }
                                yy++;

                            }

                            for (int i = 0; i < 6; i++)
                            {
                                if (ff < minibiz.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)minibiz.asilList[ff];

                                    if (tempx.durak > maxx)
                                    {
                                        maxx = tempx.durak;
                                    }
                                }
                                ff++;

                            }


                            bekleme_süresix += maxx * 5;
                            maxx = 0;
                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        oits = oits / 18;
                        Console.WriteLine("iii) 2 Araca random dağıtım için oits = " + oits);
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                    }
                    Console.WriteLine("    Yolcunun ismi        " + "  ---ITS 1---   " + "    ---ITS 2---     " + "    ---ITS 3---   " + " Süre Kazancı ");
                    foreach (yolcu yolunda in yolcuxlist)
                    {
                        double a = yolunda.its1 - yolunda.its2;
                        double b = yolunda.its1 - yolunda.its3;
                        if (a > 0)
                        {
                            Console.Write("       " + yolunda.name);
                            Console.Write("                   " + yolunda.its1);
                            Console.Write("                " + yolunda.its2);
                            Console.Write("                " + yolunda.its3);
                            Console.Write("          " + a);

                        }
                        else if (b > 0)
                        {
                            Console.Write("       " + yolunda.name);
                            Console.Write("                   " + yolunda.its1);
                            Console.Write("                " + yolunda.its2);
                            Console.Write("                " + yolunda.its3 + "   ");
                            Console.Write("                       " + b);

                        }
                        else
                        {
                            continue;
                        }

                        Console.WriteLine();


                    }







                }
                if (program_say == 2)
                {

                    minoits mino = new minoits();
                    double minoits = 90000;

                    Console.WriteLine("Hesaplama yapılıyor lütfen bekleyiniz.");
                    for (int ç = 0; ç < 100000; ç++)
                    {
                        string[] isimler = new string[] { "Selda", "Aslı", "Uras", "Deniz", "Arda", "Büşra", "Burak", "Sinem", "Arun", "Boran", "Nisa", "Daniel", "Keke", "Silay", "Harun", "Liya", "Biran", "Doğukan", "İbrahim", "Seren", "Enez", "Esin", "Ahri", "Nida", "Nazır", "Hatice", "Seçkin" };
                        int[] dizi = new int[] { 6, 3, 9, 7, 12, 10, 11, 2, 3, 8, 5, 6, 4, 1, 5, 7, 6, 3 };
                        List<yolcu> yolcuxlist = new List<yolcu>();

                        for (int i = 0; i < 18; i++)
                        {
                            yolcu yolunda_gerek = new yolcu(isimler[i], dizi[i]);
                            yolcuxlist.Add(yolunda_gerek);

                        }
                        //Console.WriteLine("------------------------ Taksi ve Minibüs Çalıştığında------------------------");
                        MinibusFIFO minibiz = new MinibusFIFO();
                        TaksiPQ takzi = new TaksiPQ();
                        List<yolcu> yolculist = yolcuxlist;




                        while (yolculist.Count != 0)//aracın kaç kere yolcu alacağını belirleyen (gidişsay)
                        {

                            for (int i = 0; i < 2; i++)
                            {
                                if ((takzi.süre % 2250 == 0) && (minibiz.süre % 6000 == 0))//aynı anda durakta olma durumları random dağıtım yapılır.
                                {

                                    for (int j = 0; j <= yolculist.Count; j++)// minibüs ve taksi aynı anda işyerinden çalışanları almaya geldilerse işçiler araçlara random olarak biner.
                                    {

                                        if (yolculist.Count != 0)
                                        {
                                            yolcu yolcux = (yolcu)yolculist[0];
                                            int a = rnd.Next(0, 2);
                                            if (a == 0 && takzi.arac_dolu_mu() == false)
                                            {
                                                takzi.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }
                                            else if (a == 1 && minibiz.arac_dolu_mu() == false)
                                            {
                                                minibiz.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }
                                            else if (minibiz.arac_dolu_mu() == false)
                                            {
                                                minibiz.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }

                                            else if (takzi.arac_dolu_mu() == false)
                                            {
                                                takzi.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);

                                            }
                                            else
                                            {
                                                break;//Kimse kalmadı binada
                                            }

                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    takzi.icerdekiler.Clear();
                                    takzi.gidis_say++;
                                    minibiz.icerdekiler.Clear();
                                    minibiz.gidis_say++;
                                }
                                if (takzi.süre % 2250 == 0)//durakta o an sadece taksi geldiyse
                                {
                                    int m = -1;
                                    for (int j = 0; j < yolculist.Count; i++)
                                    {
                                        yolcu yolcux = (yolcu)yolculist[j];
                                        if (takzi.arac_dolu_mu() == false)
                                        {
                                            if (yolcux.durak > m)////////
                                            {
                                                m = yolcux.durak;
                                            }///
                                            takzi.icerdekilere_ekle(yolcux);//icerdekilere ekle fonksiyonu sayesinde  yolcuları asil listeye ekleriz.
                                            yolculist.Remove(yolcux);
                                        }
                                        else
                                        {
                                            takzi.süre += (12 - m) * 18.75;//////////////
                                            break;
                                        }

                                    }
                                    takzi.icerdekiler.Clear();
                                    takzi.gidis_say++;


                                }
                                if (minibiz.süre % 6000 == 0)//sadece minibüs ilk duraktayken
                                {
                                    for (int j = 0; j < yolculist.Count; i++)
                                    {
                                        yolcu yolcux = (yolcu)yolculist[j];
                                        if (minibiz.arac_dolu_mu() == false)
                                        {
                                            minibiz.icerdekilere_ekle(yolcux);
                                            yolculist.Remove(yolcux);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    minibiz.icerdekiler.Clear();
                                    minibiz.gidis_say++;

                                }
                                minibiz.süre += 15;////////////////
                                takzi.süre += 15;////////
                            }

                        }
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (takzi.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 4; i++)
                            {
                                if (y < takzi.asilList.Count)
                                {
                                    yolcu temp = (yolcu)takzi.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 3.75);
                                    temp.its3 = p;
                                    oits += p;

                                    // Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 4; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < takzi.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)takzi.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 3.75;
                            max = 0;
                        }


                        double bekleme_süresix = 0;
                        int maxx = 0;
                        int yy = 0, ff = 0;

                        for (int j = 0; j < (minibiz.gidis_say); j++)//minibüs itsleri yazdırır.
                        {


                            for (int i = 0; i < 6; i++)
                            {
                                if (yy < minibiz.asilList.Count)
                                {
                                    yolcu temp = (yolcu)minibiz.asilList[yy];
                                    double p = bekleme_süresix + (temp.durak * 5);
                                    temp.its3 = p;
                                    oits += p;

                                    // Console.WriteLine(temp.ToString() + " ITS =  " + p);
                                }
                                yy++;

                            }

                            for (int i = 0; i < 6; i++)
                            {
                                if (ff < minibiz.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)minibiz.asilList[ff];

                                    if (tempx.durak > maxx)
                                    {
                                        maxx = tempx.durak;
                                    }
                                }
                                ff++;

                            }


                            bekleme_süresix += maxx * 5;
                            maxx = 0;
                        }

                        oits = oits / 18;
                        if (oits < minoits)
                        {
                            mino.minsüre = oits;
                            mino.liste.Clear();
                            mino.liste.Insert(0, takzi.asilList);
                            mino.liste.Insert(1, minibiz.asilList);
                            minoits = oits;
                        }


                    }
                    Console.WriteLine("                      ");
                    Console.WriteLine("         ");
                    Console.WriteLine("          ");
                    Console.WriteLine("           ");
                    int c = 0;
                    foreach (List<yolcu> asilliste in mino.liste)
                    {
                        if (c == 0)
                        {
                            Console.WriteLine("Taksiyle gidenler");


                        }
                        else
                        {
                            Console.WriteLine("Minibüsle gidenler");
                        }
                        foreach (yolcu yolcu in asilliste)
                        {


                            Console.WriteLine(yolcu.ToString() + " ITS =  " + yolcu.its3
    );


                        }
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();


                        c++;
                    }
                    Console.WriteLine();
                    Console.WriteLine("min oits = " + minoits);
                    
                }
                if (program_say == 3)//ortalama işlem tamamlama sürelerini yazdırır.
                {
                    string[] isimler = new string[] { "Selda", "Aslı", "Uras", "Deniz", "Arda", "Büşra", "Burak", "Sinem", "Arun", "Boran", "Nisa", "Daniel", "Keke", "Silay", "Harun", "Liya", "Biran", "Doğukan", "İbrahim", "Seren", "Enez", "Esin", "Ahri", "Nida", "Nazır", "Hatice", "Seçkin" };

                    List<yolcu> yolcuxlist = new List<yolcu>();

                    for (int i = 0; i < 25; i++)
                    {
                        int a = rnd.Next(1, 41);
                        yolcu yolunda_gerek = new yolcu(isimler[i], a);
                        yolcuxlist.Add(yolunda_gerek);

                    }



                    //a1.bölüm
                    int bölüm_say = 1;
                    if (bölüm_say == 1)
                    {
                        Console.WriteLine("------------------------Sadece Minibüs Çalıştığında------------------------");
                        MinibusFIFO minibüs = new MinibusFIFO();
                        int yolcu_say = yolcuxlist.Count;
                        while (!(yolcu_say == 0))
                        {
                            for (int i = 0; i < 6; i++)//taksiye gidiş sayısı bulduracak
                            {
                                if (yolcu_say != 0)
                                {
                                    yolcu_say--;
                                }

                            }
                            minibüs.gidis_say++;
                        }
                        minibüs.asilList = yolcuxlist;
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (minibüs.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 6; i++)
                            {
                                if (y < minibüs.asilList.Count)
                                {
                                    yolcu temp = (yolcu)minibüs.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 5);
                                    temp.its1 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 6; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < minibüs.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)minibüs.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 5;
                            max = 0;

                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        oits = oits / 18;
                        Console.WriteLine("i)Sadece minibüs çalıştığında oits = " + oits);
                        Console.WriteLine("          ");
                        Console.WriteLine("           ");


                    }

                    bölüm_say++;
                    //a2.bölüm
                    if (bölüm_say == 2)
                    {
                        Console.WriteLine("------------------------Sadece Taksi Çalıştığında------------------------");
                        TaksiPQ takzi = new TaksiPQ();
                        int yolcu_say = yolcuxlist.Count;
                        while (!(yolcu_say == 0))
                        {
                            for (int i = 0; i < 4; i++)//taksiye gidiş sayısı bulduracak
                            {
                                if (yolcu_say != 0)
                                {
                                    yolcu_say--;
                                }

                            }
                            takzi.gidis_say++;
                        }
                        foreach (yolcu a in yolcuxlist)
                        {
                            takzi.icerdekilere_ekle(a);
                        }
                        takzi.icerdekiler.Clear();
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (takzi.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 4; i++)
                            {
                                if (y < takzi.asilList.Count)
                                {
                                    yolcu temp = (yolcu)takzi.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 3.75);
                                    temp.its2 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 4; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < takzi.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)takzi.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 3.75;
                            max = 0;

                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        oits = oits / 18;
                        Console.WriteLine("i)Sadece taksi çalıştığında oits = " + oits);
                        Console.WriteLine("          ");
                        Console.WriteLine("           ");


                    }
                    bölüm_say++;


                    //a3.bölüm
                    if (bölüm_say == 3)
                    {
                        Console.WriteLine("------------------------ Taksi ve Minibüs Çalıştığında------------------------");
                        MinibusFIFO minibiz = new MinibusFIFO();
                        TaksiPQ takzi = new TaksiPQ();
                        List<yolcu> yolculist = new List<yolcu>();
                        foreach (yolcu yolc in yolcuxlist)
                        {
                            yolculist.Add(yolc);
                        }




                        while (yolculist.Count != 0)//aracın kaç kere yolcu alacağını belirleyen (gidişsay)
                        {

                            for (int i = 0; i < 2; i++)
                            {
                                if ((takzi.süre % 225 == 0) && (minibiz.süre % 300 == 0))//aynı anda durakta olma durumları random dağıtım yapılır.
                                {

                                    for (int j = 0; j <= yolculist.Count; j++)// minibüs ve taksi aynı anda işyerinden çalışanları almaya geldilerse işçiler araçlara random olarak biner.
                                    {

                                        if (yolculist.Count != 0)
                                        {
                                            yolcu yolcux = (yolcu)yolculist[0];
                                            int a = rnd.Next(0, 2);
                                            if (a == 0 && takzi.arac_dolu_mu() == false)
                                            {
                                                takzi.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }
                                            else if (a == 1 && minibiz.arac_dolu_mu() == false)
                                            {
                                                minibiz.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }
                                            else if (minibiz.arac_dolu_mu() == false)
                                            {
                                                minibiz.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);
                                            }

                                            else if (takzi.arac_dolu_mu() == false)
                                            {
                                                takzi.icerdekilere_ekle(yolcux);
                                                yolculist.Remove(yolcux);

                                            }
                                            else
                                            {
                                                break;//Kimse kalmadı binada
                                            }

                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    takzi.icerdekiler.Clear();
                                    takzi.gidis_say++;
                                    minibiz.icerdekiler.Clear();
                                    minibiz.gidis_say++;
                                }
                                if (takzi.süre % 2250 == 0)//durakta o an sadece taksi geldiyse////////////
                                {
                                    int m = -1;
                                    for (int j = 0; j < yolculist.Count; i++)
                                    {

                                        yolcu yolcux = (yolcu)yolculist[j];
                                        if (takzi.arac_dolu_mu() == false)
                                        {
                                            takzi.icerdekilere_ekle(yolcux);//icerdekilere ekle fonksiyonu sayesinde  yolcuları asil listeye ekleriz.
                                            if (yolcux.durak > m)////////
                                            {
                                                m = yolcux.durak;
                                            }////////////
                                            yolculist.Remove(yolcux);
                                        }
                                        else
                                        {
                                            takzi.süre += (40 - m) * 56.25;//////////////
                                            break;
                                        }

                                    }
                                    takzi.icerdekiler.Clear();
                                    takzi.gidis_say++;


                                }
                                if (minibiz.süre % 6000 == 0)//sadece minibüs ilk duraktayken////////////////
                                {
                                    for (int j = 0; j < yolculist.Count; i++)
                                    {
                                        yolcu yolcux = (yolcu)yolculist[j];
                                        if (minibiz.arac_dolu_mu() == false)
                                        {
                                            minibiz.icerdekilere_ekle(yolcux);
                                            yolculist.Remove(yolcux);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    minibiz.icerdekiler.Clear();
                                    minibiz.gidis_say++;

                                }
                                minibiz.süre += 4.5;////////////////
                                takzi.süre += 4.5;////////
                            }

                        }
                        double bekleme_süresi = 0;
                        int max = 0;
                        int y = 0, f = 0;
                        double oits = 0;

                        for (int j = 0; j < (takzi.gidis_say); j++)//taksi itsleri yazdırır.
                        {


                            for (int i = 0; i < 4; i++)
                            {
                                if (y < takzi.asilList.Count)
                                {
                                    yolcu temp = (yolcu)takzi.asilList[y];
                                    double p = bekleme_süresi + (temp.durak * 3.75);
                                    temp.its3 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);

                                }
                                y++;

                            }

                            for (int i = 0; i < 4; i++)//hesaplama için bölümün max numaralı durakını alır
                            {
                                if (f < takzi.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)takzi.asilList[f];

                                    if (tempx.durak > max)
                                    {
                                        max = tempx.durak;
                                    }
                                }
                                f++;

                            }


                            bekleme_süresi += max * 3.75;
                            max = 0;
                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        Console.WriteLine("          ");
                        Console.WriteLine("           ");

                        double bekleme_süresix = 0;
                        int maxx = 0;
                        int yy = 0, ff = 0;

                        for (int j = 0; j < (minibiz.gidis_say); j++)//minibüs itsleri yazdırır.
                        {


                            for (int i = 0; i < 6; i++)
                            {
                                if (yy < minibiz.asilList.Count)
                                {
                                    yolcu temp = (yolcu)minibiz.asilList[yy];
                                    double p = bekleme_süresix + (temp.durak * 5);
                                    temp.its3 = p;
                                    oits += p;

                                    Console.WriteLine(temp.ToString() + " ITS =  " + p);
                                }
                                yy++;

                            }

                            for (int i = 0; i < 6; i++)
                            {
                                if (ff < minibiz.asilList.Count)
                                {
                                    yolcu tempx = (yolcu)minibiz.asilList[ff];

                                    if (tempx.durak > maxx)
                                    {
                                        maxx = tempx.durak;
                                    }
                                }
                                ff++;

                            }


                            bekleme_süresix += maxx * 5;
                            maxx = 0;
                        }
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                        oits = oits / 18;
                        Console.WriteLine("iii) 2 Araca random dağıtım için oits = " + oits);
                        Console.WriteLine("                      ");
                        Console.WriteLine("         ");
                    }
                    Console.WriteLine("    Yolcunun ismi        " + "  ---ITS 1---   " + "    ---ITS 2---     " + "    ---ITS 3---      " + " Süre Kazancı ");
                    foreach (yolcu yolunda in yolcuxlist)
                    {
                        double a = yolunda.its1 - yolunda.its2;
                        double b = yolunda.its1 - yolunda.its3;
                        if (a > 0)
                        {
                            Console.Write("       " + yolunda.name);
                            Console.Write("                   " + yolunda.its1);
                            Console.Write("                " + yolunda.its2);
                            Console.Write("                " + yolunda.its3);
                            Console.Write("          " + a);

                        }
                        else if (b > 0)
                        {
                            Console.Write("       " + yolunda.name);
                            Console.Write("                   " + yolunda.its1);
                            Console.Write("                " + yolunda.its2);
                            Console.Write("                " + yolunda.its3 + "   ");
                            Console.Write("                       " + b);

                        }
                        else
                        {
                            continue;
                        }

                        Console.WriteLine();


                    }








                }
                if (program_say == 4)
                {
                    Randomm rnnd = new Randomm();
                    string[] isimler = new string[] { "Selda", "Aslı", "Uras", "Deniz", "Arda", "Büşra", "Burak", "Sinem", "Arun", "Boran", "Nisa", "Daniel", "Keke", "Silay", "Harun", "Liya", "Biran", "Doğukan", "İbrahim", "Seren", "Enez", "Esin", "Ahri", "Nida", "Nazır", "Hatice", "Seçkin" };
                    List<yolcu> yolcuxlist = new List<yolcu>();
                    Program program = new Program();
                    List<chromosome> kromozomlist = new List<chromosome>();

                    for (int i = 0; i < 25; i++)//random yerlere gidecek random 25 kişi oluşturulur.
                    {
                        int a = rnnd.Next(1, 41);
                        yolcu yolunda_gerek = new yolcu(isimler[i], a);
                        yolcuxlist.Add(yolunda_gerek);

                    }

                    for (int j = 0; j < 30; j++)//baslangıc kromozomlarını oluşturur.
                    {
                        int[] baslangic_kromozomları = new int[25];
                        for (int i = 0; i < 25; i++)//random yerlere gidecek random 25 kişi oluşturulur.
                        {
                            int a = rnnd.Next(0, 2);
                            baslangic_kromozomları[i] = a;


                        }

                        chromosome kromozom = new chromosome(baslangic_kromozomları, yolcuxlist);
                        kromozomlist.Add(kromozom);
                    }
                    Console.WriteLine("Genetik algoritmayı çalıştırmak istediğiniz sayıyı (üretmek istediğiniz nesil sayısını) giriniz. (örn. 50)");
                    int deneme = Convert.ToInt32(Console.ReadLine());
                    kromozomlist = program.ComputeFitness(kromozomlist, yolcuxlist);// kromozomları uygunluk fonksiyonuna gönderir
                    for (int i = 0; i < deneme; i++)
                    {



                        kromozomlist = program.Crossover(kromozomlist, rnnd, yolcuxlist);//kromozomları çaprazlamaya gönderir.

                        kromozomlist = program.Mutation(kromozomlist, rnnd, yolcuxlist);//kromozomları mutasyona tabi tutar.
                        kromozomlist = program.ComputeFitness(kromozomlist, yolcuxlist);//yeni nesilin uygunluğunu kontrol eder.
                    }
                    double minn = 9000;
                    foreach (chromosome kromozom in kromozomlist)
                    {
                        if (kromozom.oits < minn)
                        {
                            minn = kromozom.oits;
                        }

                    }
                    foreach (chromosome kromozom in kromozomlist)
                    {

                        if (kromozom.oits == minn)
                        {
                            for (int k = 0; k < 25; k++)
                            {
                                int mt = kromozom.dizi[k];

                                if (mt == 0)
                                {
                                    Console.WriteLine("Taksiye Biner ==> " + yolcuxlist[k] + "İts'si =  " + yolcuxlist[k].its3);
                                }
                                else
                                {
                                    Console.WriteLine("Minibüse Biner ==> " + yolcuxlist[k] + "İts'si =  " + yolcuxlist[k].its3);
                                }
                            }
                            break;
                        }


                    }
                    Console.WriteLine("Bulunan minimum oits = " + minn);


                    
                }
                if (program_say == 5)
                {
                    Console.WriteLine("Lütfen Bekleyiniz..." );
                    genetic life = new genetic();
                    genom g = life.dongu(100);
                    Program1 pro = new Program1();
                    int s = pro.Gerkeclestirme(g.binis);
                    pro.tostring();
                    double a = (double)(s / 60);
                    Console.WriteLine("Dakika cinsinden Ortalama İşlem Tamamlama Süresi:   {0:N2}" ,a );
                    Console.ReadKey();
                }

                
                
            }
            Console.ReadKey();

        }
    }
}
