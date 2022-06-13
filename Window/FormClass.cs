﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace School_tametable
{
    public partial class FormClass : Form
    {
        MainForm form;
        string Alf = "АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        string[] five;
        string[] six;
        string[] seven;
        string[] eight;
        string[] nine;
        string[] ten;
        string[] eleven;

        //Генерируем параллели классов на основании данных от пользователя
        private string[] CountClassPar(int count, string paral)
        {
            if (count == 0) 
            { 
                return new string[count];
            }
            else
            {
                string[] cpar = new string[count];
                for (int i = 0; i < cpar.Length; i++)
                {
                    cpar[i] = paral + Alf[i];

                }
                return cpar;
            }      
            
        }
      
        int getCount(string str)
        {
            return string.IsNullOrEmpty(str) ? 0 : Convert.ToInt32(str);
        }

        int getCountDb(string str)
        {
            using(schoolContext db = new schoolContext())
            {
                var nameclass = db.NameClasses.Where(p => EF.Functions.Like(p.NameClass1, str)).ToList();
                return nameclass.Count;
            }

        }

        void OpDb(int uc, int dbc, string[] mas, schoolContext db, string str)
        {
            if(uc > dbc)
            {
                foreach (string s in mas)
                {
                    if (db.NameClasses.Where(p => p.NameClass1 == s).FirstOrDefault() == null)
                    {
                        NameClass nameClass = new NameClass { NameClass1 = s };
                        db.NameClasses.Add(nameClass);
                    }

                }
            }

            if(uc<dbc)
            {
                //Пересечение ищем и исключаем то что нашли из выборки
                var lmas = db.NameClasses.Where(x => EF.Functions.Like(x.NameClass1, str) && mas.Contains(x.NameClass1)).Select(p=>p.IdNameCl).ToArray();
                var tmpnc = db.NameClasses.Where(p => EF.Functions.Like(p.NameClass1, str) && !lmas.Contains(p.IdNameCl)).ToList();

                foreach (var t in tmpnc)
                {
                   db.NameClasses.Remove(t);                  

                }
            }
        }


        public FormClass()
        {
            InitializeComponent();
        }
        public FormClass(MainForm f)
        {
            this.TopMost = true;
            form = f;
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        { try
            {
                // Значения введенные пользователем
                int f = getCount(maskedTextBox1.Text);
                int s = getCount(maskedTextBox2.Text);
                int se = getCount(maskedTextBox3.Text);
                int ei = getCount(maskedTextBox4.Text);
                int n = getCount(maskedTextBox5.Text);
                int t = getCount(maskedTextBox6.Text);
                int el = getCount(maskedTextBox7.Text);

                // Формируем массивы для записи
                five = CountClassPar(f, "5");
                six = CountClassPar(s, "6");
                seven = CountClassPar(se, "7");
                eight = CountClassPar(ei, "8");
                nine = CountClassPar(n, "9");
                ten = CountClassPar(t, "10");
                eleven = CountClassPar(el, "11");

                //Значения, хранящиеся в БД
                int fdb = getCountDb("5%");
                int sdb = getCountDb("6%");
                int sedb = getCountDb("7%");
                int eidb = getCountDb("8%");
                int ndb = getCountDb("9%");
                int tdb = getCountDb("10%");
                int eldb = getCountDb("11%");

                List<string[]> Parallel = new List<string[]> {five,six,seven,eight,nine,ten,eleven};

                using (schoolContext db = new schoolContext())
                {
                    
                    OpDb(f, fdb, Parallel[0], db, "5%");
                    OpDb(s, sdb, Parallel[1], db, "6%");
                    OpDb(se, sedb, Parallel[2], db, "7%");
                    OpDb(ei, eidb, Parallel[3], db, "8%");
                    OpDb(n, ndb, Parallel[4], db, "9%");
                    OpDb(t, tdb, Parallel[5], db, "10%");
                    OpDb(el, eldb, Parallel[6], db, "11%");

                    NameClass.TruCathcSave(db, form, this, "Запись класса уже есть в справочнике!");
                }


            }
            catch
            {
                
            }

        }

        private void FormClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Enabled = true;
            form.TopMost = true;
        }
    }
}
