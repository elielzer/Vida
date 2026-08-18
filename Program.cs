using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomDia
{
    class Program
    {
        public static char CharValue;

        public static BomDia Bomdia { get; set; }
        public static Pad pad { get; set; }
        public static string MeuAssemblyVersion
        {
            get
            {
                //Console.WriteLine(typeof(BomDia).Assembly.FullName);
                
                //return Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return typeof(BomDia).Assembly.GetName().Version.ToString();

            }
        }

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        //[MTAThread]
        //static void Main()
        // Definição do delegate
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            VariáveisGlobais.LerDados();
            try
{            foreach (DataRow rows in VariáveisGlobais.dataSetBiblioteca.Tables[0].Rows)
                {
                    string _ = rows.Field<string>("Valor").ToString();
                    switch (rows.Field<string>("Tipo").ToString())
                    {
                        case "Principal":
                            VariáveisGlobais.CaminhoDados = _;
                            break;
                        case "Pastas":
                            VariáveisGlobais.CaminhoDasPastas = _;
                            break;
                        case "BancoDados":
                            VariáveisGlobais.CaminhoBancoDeDados = _;
                            break;
                        case "Impressos":
                            VariáveisGlobais.CaminhoDosImpressos = _;
                            break;
                    }
                }
            }
            catch
            {
                return;
            }
            

            DataRow encontre_o = VariáveisGlobais.dataSetBiblioteca.Tables[0].Rows.Find(1);
            VariáveisGlobais.MyPath = encontre_o[2].ToString();
            VariáveisGlobais.info = new DirectoryInfo(VariáveisGlobais.MyPath);

            Bomdia = new BomDia();
            Application.Run(Bomdia);
        }
        public static int DiaBomDiaX = 0; public static int DiaBomDiaY = 0;

        

    }
    public static class VariáveisGlobais
    {
        public static DataSet dataSetBiblioteca;
        public static DataTable Config;
        public static DataColumn Coluna1;
        public static DataRow row;
        public static string MyPath;
        public static string MyPathForLink;

        public static string CaminhoBancoDeDados;
        public static string CaminhoDasPastas;
        public static string CaminhoDados;
        public static string CaminhoDosImpressos;

        public static DirectoryInfo nodeDirInfo; public static TreeNode newSelected;
        public static DirectoryInfo info;

        public static void CriaTabela()
        {
            dataSetBiblioteca = new DataSet(); var keys = new DataColumn[1];
            Config = new DataTable("Config");

            Coluna1 = new DataColumn
            {
                DataType = typeof(int),
                ColumnName = "Segmento",
                ReadOnly = false,
                Unique = true,
                AutoIncrement = true,
                AutoIncrementStep = 1
            };


            Config.Columns.Add(Coluna1);
            keys[0] = Coluna1;
            Config.PrimaryKey = keys;

            Coluna1 = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Tipo",
                ReadOnly = false
            };

            Config.Columns.Add(Coluna1);

            Coluna1 = new DataColumn
            {
                DataType = typeof(string),
                ColumnName = "Valor",
                ReadOnly = false
            };

            Config.Columns.Add(Coluna1);

            dataSetBiblioteca.Tables.Add(Config);

        }
        
        public static void LerDados()
        {
            VariáveisGlobais.CriaTabela();
            // Diretrizes de dados incluídos na publicação
            dataSetBiblioteca.ReadXml("bomDiaConfig.xml", XmlReadMode.ReadSchema);


        }

    }
}
