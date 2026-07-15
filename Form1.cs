using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Schema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace BomDia
{
    public partial class BomDia : Form
    {
        int LarguraForm = 0; int AlturaForm = 0;  int Xloc = 0; int Yloc = 0; int LarguraReduzida = 0;

        Pad pad; // Abre o formulário suspenso especial

        // Obter o nome de usuário do Windows
        public string Usuário = WindowsIdentity.GetCurrent().Name.ToString();

        //
        int AlturaReduzida = 0;
        int NRow = 0; // Usado na contagem de linhas no datagridview1
        DateTime DataSemana;

        string BancoDados = VariáveisGlobais.CaminhoBancoDeDados;
        
        public const char Triang = '\u25E3';
        //
        public string PréPorque = ""; public string PréQuando;


        public DateTime dataHoje; public DateTime dataPara;  public int ContadorDeClique = 0; public string Old_label = "";

        public BomDia()
        {
            InitializeComponent(); splitContainer2.Panel1Collapsed = true;

            // Restrição no comportamento de alguns controles
            OQuePretendido.Enabled = false; QuandoPrevisto.Enabled = false; ComboBoxPorque.Enabled = false; flowLayoutPanel4.Enabled = false; ButtonAnexa.Enabled = false; bindingNavigatorDeleteItem.Enabled = false;
            PictureBoxEditar.Enabled = false;
            DataGridView1.ColumnHeadersDefaultCellStyle.Font = Control.DefaultFont;
            DataGridView1.DefaultCellStyle.Font = Control.DefaultFont;
            dataGridViewPrévia.DefaultCellStyle.Font = Control.DefaultFont;

            // Associação de dados manuais
            textBox2.TextBox.DataBindings.Add("Text", TarefasBindingSource, "IND", false, DataSourceUpdateMode.OnPropertyChanged);

            tableLayoutPanel5.Hide();
        }
        public void BomDia_Load(object sender, EventArgs e)
        {
            TarefasDataSet.ReadXml(BancoDados, XmlReadMode.ReadSchema);
            //
            sender = this.ListaDeDatas;

            // Como esteja a janela do aplicativo 
            DataHoje.Visible = true;   DataHoje.Text = DateTime.Today.ToShortDateString();

            Xloc = Location.X; Yloc = Location.Y;  LarguraForm = Width; AlturaForm = Height;

            // Layout mini janela
            //Program.DiaBomDiaX = this.Location.X + this.Width / 2 + 70;
            //Program.DiaBomDiaY = this.Location.Y + 20;


            //FormBorderStyle = FormBorderStyle.None;
            //StatusStripBomDia.Visible = false;
            LarguraReduzida =
                (int)(((tableLayoutPanel2.Width * 1.03)));

            AlturaReduzida =
                (int)(tableLayoutPanel2.Height * 1.05);

            //Width = LarguraReduzida;
            //this.BackColor = Color.Black; this.BackColor = Color.Black;
            //this.SemanaToolStripButton.ForeColor = Color.Yellow;
            // Controles invisíveis
            DetalheUsuário.Hide(); PastaOculto.Hide(); label1.Text = "Desabilitado"; Old_label = label1.Text;
            //BindingNavegador.Hide(); 
            Responsável.Hide();


            // Transição altura da mini janela
            //splitContainer1.Panel2Collapsed = true;  
            //Height = AlturaReduzida;

            //Height = AlturaReduzida + DataHoje.Height;  
            timer2.Enabled = true; timer2.Stop(); timer2.Start();

            // Mostra a data do dia
            //Location = new Point(1050, 0);
            ListaDeDatas.Text = DateTime.Today.ToShortDateString();

            // Selecionar dados a apresentar conforme critério
            TarefasBindingSource.Filter =  String.Format("QUANDO = '{0:dd/MM/yyyy}'", ListaDeDatas.Text);

            // Formatação de datas
            string SemanaComMaiuscula; SemanaComMaiuscula = DateTime.Today.ToString("ddd");
            SemanaComMaiuscula = SemanaComMaiuscula[0].ToString().ToUpper() +
                SemanaComMaiuscula[1].ToString() + SemanaComMaiuscula[2].ToString();

            SemanaToolStripButton.Text = string.Concat(".", SemanaComMaiuscula);

            // Carregar tabela de configuração para dentro da grade
            this.dataGridView3.DataSource = VariáveisGlobais.dataSetBiblioteca;
            this.dataGridView3.DataMember = VariáveisGlobais.dataSetBiblioteca.Tables[0].ToString();

            // Carregar dados para a prévia
            FiltraDadosPrévia();
            checkBoxExibirPrévia.Text = "Prévia ligado";
            checkBoxExibirPrévia.Refresh();


            // Mostrar dados marcados
            FiltraDadosMarcados();

            // Preencho o nome do usuário na barra de status
            toolStripStatusLabelUsuário.Text = "Usuário atual: ".ToUpper() + Usuário.ToUpper();


            //Classificar dados
            string rowFilter = "QUANDO > '" + DateTime.Today + "'";
            bindingSourcePrévia.Filter = rowFilter;
            bindingSourcePrévia.Sort = "QUANDO";
            dataGridViewPrévia.FirstDisplayedScrollingRowIndex = 0;
            ;
        }

        public void FiltraDadosPrévia()
        {

            // Carregar dados para visão de prévias
            string rowFilter = "QUANDO > '" + DateTime.Today + "'";
            bindingSourcePrévia.Filter = rowFilter;
            bindingSourcePrévia.Sort = "QUANDO";
            dataGridViewPrévia.FirstDisplayedScrollingRowIndex = 0;
 
        }

        public void FiltraDadosMarcados()
        {

            // Carregar dados para visão de marcados
            try
            {
            string rowFilter = "QUANDO = '" + DateTime.Today + "' AND DIAMARCADO = QUANDO";
            
            bindingSourceMarcados.Filter = rowFilter;
            ;
            
            dataGridViewMarcados.FirstDisplayedScrollingRowIndex = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }


        private void CortinaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Surgir a janela principal no tamanho padrão
            splitContainer1.Panel2Collapsed = false;

            StatusStripBomDia.Show(); Location = new Point(Xloc, Yloc); Width = LarguraForm; Height = AlturaForm;

            this.BackColor = DefaultBackColor; this.WindowState = FormWindowState.Normal; FormBorderStyle = FormBorderStyle.Sizable;
            ControlBox = true; cortinaToolStripMenuItem.Enabled = false; BindingNavegador.Visible = true;
            SemanaToolStripButton.ForeColor = Color.Black;
            /* Mostra a data*/
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Font =  new Font("Agency FB", 10F, FontStyle.Regular);

            //atualiza data tempo real com base no sistema operacional
            if (ListaDeDatas.Text != DateTime.Today.ToShortDateString())
            {
                ListaDeDatas.Text = DateTime.Today.ToShortDateString();
            }
            this.Tag = "Max";

        }
        public enum ValorTag
        {
            Min
        }
        public void BomDia_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (e.KeyChar == (char)27)
            {
                // Zerar a variável global 
                Program.CharValue = (char)0;

                var EstaTag = this.Tag;
                switch (EstaTag)
                {
                    case "Min":
                        return;

                }

                if (pad != null)
                { pad.Dispose(); }
                Program.Bomdia.TopMost = true;


                // Menu suspenso para o comando Voltar.

                StatusStripBomDia.Visible = false;
                this.WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                Xloc = Location.X; Yloc = Location.Y; 
                LarguraForm = Width;
                AlturaForm = Height;
                Width = LarguraReduzida;


                /* Muda formato */
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "HH:mm";
                dateTimePicker1.Font = new Font("Calibri", 22F, FontStyle.Bold);

                Height = (int)(((ushort)dateTimePicker1.Height) * 1.05);
                this.BackColor = Color.Black;
                this.SemanaToolStripButton.ForeColor = Color.Yellow;
                Location = new Point(1100, 0);
                cortinaToolStripMenuItem.Enabled = true;
                this.Tag = "Min";

            }
            ;

        }

        private void SalvarToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                TarefasBindingSource.EndEdit();
                TarefasDataSet.AcceptChanges();

                //
                if (TarefasDataSet == null) { return; }

                // Create a file name to write to.
                string filename = VariáveisGlobais.CaminhoBancoDeDados;

                // Create the FileStream to write with.
                System.IO.FileStream stream = new System.IO.FileStream
                    (filename, System.IO.FileMode.Create);

                // Create an XmlTextWriter with the fileStream.
                System.Xml.XmlTextWriter xmlWriter =
                    new System.Xml.XmlTextWriter(stream,
                    System.Text.Encoding.Unicode);

                // Write to the file with the WriteXml method.
                TarefasDataSet.WriteXml(xmlWriter);
                xmlWriter.Close();
                MSGtoolStripStatusLabel.Text = "Anexada a tarefa no arquivo XML " +
                    DateTime.Now.ToString();

                this.PictureBoxEditar.Image = global::Vida.Properties.Resources.CLIP07;
            }
            catch
            {
            }
        }
        // Entrada de novo registro, manualmente, ao arquivo XML ----------------
        private void BindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            System.DateTime dTime = DateTime.Today;
            System.TimeSpan tSpan = new System.TimeSpan(1, 0, 0, 0);
            dTime = dTime + tSpan;
            PréPorque = "A";

            if (dataPara > dataHoje)
            {
                //dataPara = dataHoje;
                PréQuando = dTime.ToShortDateString();
                //PréQuando = DateTime.Today.ToShortDateString();
            }
            else
            {
                PréQuando = DateTime.Today.ToShortDateString();

            }

            QuandoPrevisto.Text = PréQuando;
            ComboBoxPorque.SelectedValue = PréPorque;

            DetalheUsuário.DataBindings.Control.Text = Usuário;

            ActiveControl = OQuePretendido;
            MSGtoolStripStatusLabel.Text = "Esboço...";
            this.PictureBoxEditar.Image =
                global::Vida.Properties.Resources.NEW;
            label1.Text = "Novo item..."; label5.Text = "";
            Old_label = label1.Text;
            bindingNavigatorAddNewItem.Enabled = false;
        }

        private void abrirToolStripButton_Click(object sender, EventArgs e)
        {
            //tableLayoutPanel6.Enabled = true;
        }


        private void CheckBoxIntegrador_Click(object sender, EventArgs e)
        {
            if (CheckBoxIntegrador.Checked == true)
            {
                TarefasBindingSource.Filter = String.Format("QUANDO = '{0:dd/MM/yyyy}'", ListaDeDatas.Text);
            }
            else
            {
                TarefasBindingSource.Filter = "";
            }
        }

        private void ListaDeDatas_SelectedValueChanged(object sender, EventArgs e)
        {
            dataHoje = DateTime.Today;
            dataPara = Convert.ToDateTime(ListaDeDatas.SelectedItem);
            if (CheckBoxIntegrador.Checked)
            {
                TarefasBindingSource.Filter = String.Format("QUANDO = '{0:dd/MM/yyyy}'",
                    ListaDeDatas.Text);
            }
            //
            if (dataPara > dataHoje)
            {
                DiaBomDiaLabel.Text = "Programático".ToUpper();
                this.DiaBomDiaLabel.BackgroundImage = null;
                bindingNavigatorAddNewItem.Text = "&Criar";
                if (bindingNavigatorAddNewItem.Text != "&Criar")
                { bindingNavigatorAddNewItem.Text = "&Criar"; }

                if (bindingNavigatorAddNewItem.Enabled == false)
                { bindingNavigatorAddNewItem.Enabled = true; }
            }
            if (dataPara < dataHoje)
            {
                DiaBomDiaLabel.Text = "Em log".ToUpper();
                this.DiaBomDiaLabel.BackgroundImage = null;
                if (bindingNavigatorAddNewItem.Enabled != false)
                { bindingNavigatorAddNewItem.Enabled = false; }
            }
            if (dataPara == dataHoje)
            {
                MSGtoolStripStatusLabel.Text =
                    "Arquivo de dados: " + BancoDados;

                DiaBomDiaLabel.Text = "Em Pauta".PadLeft(10);
                groupBox2.Text = "Tempo Real".PadLeft(15);
                //global::BomDia.Properties.Resources.Edit1;
                this.DiaBomDiaLabel.BackgroundImage = global::Vida.Properties.Resources.MOON05;
                if (bindingNavigatorAddNewItem.Enabled == false)
                { bindingNavigatorAddNewItem.Enabled = true; }
                if (bindingNavigatorAddNewItem.Text != "&Inserir")
                { bindingNavigatorAddNewItem.Text = "&Inserir"; }
            }
            VariaveisGlobais.ListaDeDatasText = ListaDeDatas.Text;

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {

            dateTimePicker1.ResetText();

            string SemanaComMaiuscula = DateTime.Today.ToString("ddd");
            SemanaComMaiuscula = SemanaComMaiuscula[0].ToString().ToUpper() +
                SemanaComMaiuscula[1].ToString() + SemanaComMaiuscula[2].ToString();
            SemanaToolStripButton.Text = string.Concat(".", SemanaComMaiuscula);
        }

        // Tornar os dados de um registro anterior como o registro para nova data
        private void MigrarToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < NRow; i++)
            {

                switch (DataGridView1.Rows[i].Cells[8].Value)
                {
                    case true:

                        DataRow Row;
                        Row = BomDiaTarefas.NewRow();
                        Row["QUANDO"] = DateTime.Today.ToShortDateString();
                        Row["OQUE"] = DataGridView1.Rows[i].Cells[1].Value; //4ª col
                        Row["PORQUE"] = DataGridView1.Rows[i].Cells[4].Value; //5ª col


                        Row["PESO"] = DataGridView1.Rows[i].Cells[5].Value; //6ª col
                        Row["CRITÉRIO"] = DataGridView1.Rows[i].Cells[6].Value; //7ª col
                        Row["DIAMARCADO"] = DataGridView1.Rows[i].Cells[3].Value; //3ª col

                        // Índice
                        BomDiaTarefas.Rows.Add(Row);
                        MSGtoolStripStatusLabel.Text =
                            "Migrado IND(" + DataGridView1.Rows[i].Cells[0].Value + ")" + " para hoje";

                        break;
                    case false:
                        ;
                        break;
                }

            }

        }
        public void VoltarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* Formata o campo para hora */
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm";
            dateTimePicker1.Font = new Font("Calibri", 22F, FontStyle.Bold);

            // Redesenhar a janela para forma reduzida
            this.WindowState = FormWindowState.Normal;
            StatusStripBomDia.Visible = false;
            FormBorderStyle = FormBorderStyle.None;  Xloc = Location.X; Yloc = Location.Y;
            LarguraForm = Width; AlturaForm = Height;

            Width = LarguraReduzida;

            Height = (int)(((ushort)dateTimePicker1.Height) * 1.1);
            this.BackColor = Color.Black; Location = new Point(1100, 0);
            this.SemanaToolStripButton.ForeColor = Color.Yellow;

            cortinaToolStripMenuItem.Enabled = true;

            Program.Bomdia.TopMost = true; HideForm();// Ocultar 
        }
        private void DataGridView1_Enter(object sender, EventArgs e)
        {
            MSGtoolStripStatusLabel.Text = "Bom dia. Arquivo de dados: " + TarefasDataSet.Namespace;
            NRow = DataGridView1.RowCount - 1;
            this.Text = Application.ProductName + " " + NRow.ToString() + "Tasks";
        }

        private void hojeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListaDeDatas.Text = DateTime.Today.ToShortDateString();
        }

        //-----------------------------------------------------------------------------
        // Escreve as mudanças dinâmicas de data de execução de tarefas para o interface
        //-----------------------------------------------------------------------------
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {


            this.PictureBoxEditar.Image = global::Vida.Properties.Resources.Edit1;
            //this.label1.Text = (string)"Prompt";
            if (DataGridView1.CurrentRow == null) //quando a posição está em linha nova
            {
                try
                {
                    MonthCalendarDiamarcado.SelectionRange = new SelectionRange(
                      lower: DateTime.Today,
                      upper: DateTime.Today); //marcação padrão na data de hoje

                    label5.Text = "(em branco)".ToUpper(); //texto fantasia
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return;
            } // Finaliza em seguida aqui
            //
            else
                // Redefine o texto atual da barra de status
                MSGtoolStripStatusLabel.Text = "Item: " + DataGridView1.CurrentRow.Cells[0].Value;

            if (DataGridView1.CurrentRow.Cells[3].Value.ToString() == "")
            {
                try
                {
                    MonthCalendarDiamarcado.SelectionRange = new SelectionRange(
                      lower: DateTime.Today,
                      upper: DateTime.Today);
                    if (DataGridView1.CurrentRow.Cells[3].Value.ToString() == "")
                    {
                        label5.Text = "☀" + " " + "agenda do dia".ToUpper();
                        return;
                    }
                    else
                    {
                        DataSemana = (DateTime)DataGridView1.CurrentRow.Cells[3].Value;
                        if (DataSemana == DateTime.Today)
                        {
                            label5.Text = "Hoje".ToUpper();
                            return;
                        }
                        else
                        {
                            label5.Text = "";
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return; // finaliza se erro
            }
            else  // Quando o registro já existe fazer isso.
            {
                //
                try
                {
                    MonthCalendarDiamarcado.SelectionRange = new SelectionRange(
                      lower: (DateTime)DataGridView1.CurrentRow.Cells[3].Value,
                      upper: (DateTime)DataGridView1.CurrentRow.Cells[3].Value);
                    DataSemana = (DateTime)DataGridView1.CurrentRow.Cells[3].Value;
                    //
                    if (DataSemana == DateTime.Today)
                    {
                        label5.Text = "⛳ Previsto para hoje".ToUpper();
                        return;
                    }
                    else
                    {

                        switch (DataSemana.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                label5.Text = "segunda-feira".ToUpper();
                                break;

                            case DayOfWeek.Tuesday:
                                label5.Text = "terça-feira".ToUpper();
                                break;

                            case DayOfWeek.Wednesday:
                                label5.Text = "quarta-feira".ToUpper();
                                break;
                            case DayOfWeek.Thursday:
                                label5.Text = "quinta-feira".ToUpper();
                                break;
                            case DayOfWeek.Friday:
                                label5.Text = "sexta-feira".ToUpper();
                                break;
                            case DayOfWeek.Saturday:
                                label5.Text = "⮡ sábado".ToUpper();
                                break;
                            case DayOfWeek.Sunday:
                                label5.Text = "domingo".ToUpper();
                                break;
                            default:
                                label5.Text = "Não definida".ToUpper();
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void SemanaToolStripButton_MouseHover(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Short;

            dateTimePicker1.Font = new Font("Agency FB", 10F, FontStyle.Regular);

        }

        private void SemanaToolStripButton_MouseLeave(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm";
            dateTimePicker1.Font = new Font("Calibri", 22F, FontStyle.Bold);

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {

            //Height = AlturaReduzida;
            DataHoje.Visible = false;
            timer2.Stop();

            // definir a propriedade de um menu suspenso.
            dateTimePicker1.ContextMenuStrip = ChaveadorContextMenuStrip;
        }

        private void TarefasBindingSource_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
        {
            if (PréPorque != "" & PréQuando != "")
            {
                TarefasBindingSource.CancelEdit();
                DialogResult dialogResult =
                    MessageBox.Show("Contexto incompleto", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                ; return;
            }

            label1.Text = "Escrever item";
            Old_label = label1.Text;
            this.PictureBoxEditar.Image = global::Vida.Properties.Resources.NEW;

        }

        private void DataGridView1_Leave(object sender, EventArgs e)
        {
            this.Text = Application.ProductName.ToString();
        }
        // Manipular a operação de saída de relatório
        private void PrintDataGridView(DataGridView dataGridView)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, e) => PrintPageHandler(sender, e, dataGridView);
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog
            {
                Document = printDocument
            };
            printPreviewDialog.Show();
        }

        // Constrói o método de saída de relatório
        public void PrintPageHandler(object sender, PrintPageEventArgs e, DataGridView dataGridView)
        {
            int leftMargin = e.MarginBounds.Left;
            int topMargin = e.MarginBounds.Top;
            bool morePagesToPrint = false;
            int currentY = topMargin;

            // Print the header
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                int columnWidth = column.Width;
                e.Graphics.DrawString(column.HeaderText, column.InheritedStyle.Font, Brushes.Black, leftMargin, currentY);
                leftMargin += columnWidth;
            }
            // 2*rowHeight
            currentY += 20;

            // Print the rows
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                dataGridView.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                leftMargin = e.MarginBounds.Left;
                int rowHeight = row.Height;
                foreach (DataGridViewCell cell in row.Cells)
                {

                    int cellWidth = cell.Size.Width;
                    int cellHeight = cell.Size.Height;
                    e.Graphics.DrawString(cell.EditedFormattedValue.ToString(),
                        cell.InheritedStyle.Font, Brushes.Black, leftMargin, currentY);
                    leftMargin += cellWidth;
                }
                currentY += rowHeight;

                // Check if we need to print more pages
                if (currentY + rowHeight > e.MarginBounds.Bottom)
                {
                    morePagesToPrint = true;
                    break;
                }
            }

            e.HasMorePages = morePagesToPrint;
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            DataGridView1.Refresh();
            PrintDataGridView(DataGridView1);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        }

        private void BomDia_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }


        private void ButtonMoveLista_Click(object sender, EventArgs e)
        {
            int NCaseTrue = 0;
            try
            {
                string MsgTexto = "";
                for (int i = 0; i < NRow; i++)
                {
                    switch (DataGridView1.Rows[i].Cells[8].Value)
                    {
                        case true:
                            NCaseTrue++;
                            DataRow Row;
                            Row = BomDiaTarefas.NewRow();
                            Row["QUANDO"] = DateTime.Today.ToShortDateString();
                            Row["OQUE"] = DataGridView1.Rows[i].Cells[1].Value; //4ª col
                            Row["PORQUE"] = DataGridView1.Rows[i].Cells[4].Value; //5ª col


                            Row["PESO"] = DataGridView1.Rows[i].Cells[5].Value; //6ª col
                            Row["CRITÉRIO"] = DataGridView1.Rows[i].Cells[6].Value; //7ª col
                            Row["DIAMARCADO"] = DataGridView1.Rows[i].Cells[3].Value; //3ª col
                            Row["Pasta"] = DataGridView1.Rows[i].Cells[9].Value;
                            Row["User"] = Responsável.Text;
                            toolStripStatusLabelUsuário.Text = Responsável.Text;
                            // Índice do registro automático na adição de registro
                            BomDiaTarefas.Rows.Add(Row);


                            MsgTexto =
                                DataGridView1.Rows[i].Cells[0].Value.ToString();
                            MSGtoolStripStatusLabel.Text =
                                "Ok.";

                            break;
                        case false:
                            ;
                            break;
                    }
                    //
                }
                string message = "";
                if (NCaseTrue == 1)
                {
                    message = "Migrado item IND(" + MsgTexto + ") para hoje.";
                }
                else
                {
                    message = "Migrados " + NCaseTrue + " itens para hoje." +
                        "  Último foi IND(" + MsgTexto + ")";
                }

                const string caption = "Resgate de Tarefas";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Information);
            }
            catch { }
            ;
        }

        private void ShowLineJoin(PaintEventArgs e)
        {
            // Create pen.
            Pen bluePen = new Pen(Color.Black, 100);
            bluePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            // Create points that define line - linha superior do cabeçalho.
            PointF point1 =
                new PointF(tableLayoutPanel9.Left, tableLayoutPanel9.Bottom);
            PointF point2 =
            new PointF(PictureBoxEditar.Left, point1.Y);

            // Draw line to screen.
            e.Graphics.DrawLine(bluePen, point1, point2);
        }

        //private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        //{
        //    ShowLineJoin(e);
        //}
        private void ShowLineJoin_tableLayoutPanel9(PaintEventArgs e)
        {
            // Create pen.
            Pen bluePen = new Pen(Color.Black, 1);
            bluePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            // Create points that define line.
            PointF point1 =
                new PointF(splitContainer5.Left + textBox1.Width,
                splitContainer5.Top + splitContainer5.Height - tableLayoutPanel11.Height
               );

            PointF point2 =
            new PointF(point1.X + tableLayoutPanel9.Width, point1.Y);

            // Draw line to screen.
            e.Graphics.DrawLine(bluePen, point1, point2);
        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {
            ShowLineJoin_tableLayoutPanel9(e);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
        private void HideForm()
        {
            if (pad == null)
            {
                return;
            }
            else
            {

                pad.Activate();
                pad.Hide();

            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            DataGridView1.DataSource = TarefasBindingSource;
        }

        private void ListaDeDatas_Enter(object sender, EventArgs e)
        {
            MSGtoolStripStatusLabel.Text = "Campo: Portal";
        }

        private void AbrirPad()
        {

            if (Program.Bomdia.TopMost == true)
            {
                Program.Bomdia.TopMost = false;
            }

            if (pad == null)
            {
                Program.Bomdia.TopMost = false; pad = new Pad();
                pad.TopLevel = true;
                pad.Portal +=
                    (s, Stexto) => ListaDeDatas.Text = Stexto; // Assina o evento
                pad.Show();
                return;
            }
            else
            {
                pad.Show();
            }

        }

        private void DataGridView1_MouseEnter(object sender, EventArgs e)
        {

            if (pad == null)
            {
                pad = new Pad();

                //pad.Location =
                //    new Point(Program.DiaBomDiaX, Program.DiaBomDiaY);
                pad.Show();

            }
            else if (pad.IsDisposed)
            {
                pad = new Pad();
                //pad.Location =
                //    new Point(Program.DiaBomDiaX, Program.DiaBomDiaY);
                pad.Show();
            }

            Program.Bomdia.TopMost = false;

            pad.TopMost = true;
            pad.TopLevel = true;
            Program.Bomdia.Activate();

        }

        private void BomDia_Activated(object sender, EventArgs e)
        {
            // Desativar o formulário secundário.

            Program.Bomdia.MSGtoolStripStatusLabel.Text = "Vida.";

        }

        private void DataGridView1_MouseLeave(object sender, EventArgs e)
        {
            // Variável global
            Program.CharValue = (char)0;
        }

        private void memorizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Se o arquivo está vazio então retorna
            if (DataGridView1.RowCount == 0) { return; }
            // Senão prossegue
            // Se a data ou descrição estiver vazio então retorna
            // senão prossegue
            if ((QuandoPrevisto.Text == "") || (OQuePretendido.Text == ""))
            {
                TarefasBindingSource.CancelEdit();
                bindingNavigatorAddNewItem.Enabled = true;
                return;
            }

            try
            {
                // Pré finaliza edição 
                // conclui transpondo dados para uma tabela temporária

                TarefasBindingSource.EndEdit();
                TarefasDataSet.AcceptChanges();
                this.PictureBoxEditar.Image = global::Vida.Properties.Resources.CLIP07;
            }
            catch { return; }

            PréPorque = ""; PréQuando = "";

            // Escreve um texto de status 
            MSGtoolStripStatusLabel.Text = "Memorizado ok";

            // Se status de novo item for memorizado então habilita botão item novo
            // senão continua desabilitado

            if (bindingNavigatorAddNewItem.Enabled == false)
            {
                bindingNavigatorAddNewItem.Enabled = true;
            }


        }
        private void portalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // exibe o painel esquerdo de um contêiner
            if (splitContainer2.Panel1Collapsed == false) { splitContainer2.SplitterDistance = 86; return; }
            splitContainer2.Panel1Collapsed = false;
        }

        //private void tableLayoutPanel14_Paint(object sender, PaintEventArgs e)
        //{
        //    ShowLineJoin(e);
        //}

        private DataGridView GetDataGridView1()
        {
            return DataGridView1;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e, DataGridView dataGridView1)
        {
            {
                switch (toolStripStatusLabel1.Tag.ToString())
                {
                    case "Apagado":
                        toolStripStatusLabel1.Image = global::Vida.Properties.Resources.LIGHTON;
                        toolStripStatusLabel1.Tag = "Aceso";
                        DataGridView1.BackgroundColor = Color.White;
                        DataGridView1.DefaultCellStyle.BackColor = Color.White;
                        DataGridView1.ForeColor = Color.Black;
                        break;

                    case "Aceso":
                        toolStripStatusLabel1.Image = global::Vida.Properties.Resources.LIGHTOFF;
                        dataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(50, 10, 100);
                        DataGridView1.ForeColor = Color.White;
                        toolStripStatusLabel1.Tag = "Apagado";
                        break;
                }
            }
        }

        private void voltarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Ocultar o pad
            HideForm();
        }

        private void TarefasBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            //this.PictureBoxEditar.Image = global::BomDia.Properties.Resources.CLIP07;
        }

        public void DefinirStatus()
        {
            PictureBoxEditar.Image = global::Vida.Properties.Resources.NOTE14;
            if (label1.Text != "...")
            {
                label1.Text = "...";
                Old_label = label1.Text;
            }
        }


        private void QuandoPrevisto_TextChanged(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            PictureBoxEditar.Image = global::Vida.Properties.Resources.NOTE14;
        }

        private void OQuePretendido_KeyPress(object sender, KeyPressEventArgs e)
        {
            DefinirStatus();
        }

        private void QuandoPrevisto_KeyPress(object sender, KeyPressEventArgs e)
        {
            DefinirStatus();
        }

        private void DiaMarcadoPretendido_KeyPress(object sender, KeyPressEventArgs e)
        {
            DefinirStatus();
        }

        private void ComboBoxPorque_KeyPress(object sender, KeyPressEventArgs e)
        {
            DefinirStatus();
        }

        private void DetalheUsuário2_KeyPress(object sender, KeyPressEventArgs e)
        {
            DefinirStatus();
        }

        private void ComboBoxPorque_SelectedIndexChanged(object sender, EventArgs e)
        {
            DefinirStatus();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            ListaDeDatas.Text = dateTimePicker2.Value.ToShortDateString();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            // Aplicar um filtro de data
            //
            string rowFilter = "";

            switch (this.comboBoxCritério.Text)
            {
                case "Programático":

                    //anular o filtro atual
                    CheckBoxIntegrador.Checked = false;
                    // aplicar filtro para o programático
                    rowFilter = "QUANDO > '" + ListaDeDatas.Text + "'";
                    TarefasBindingSource.Filter = rowFilter;
                    break;
                case "Contextual":
                    //anular o filtro atual
                    CheckBoxIntegrador.Checked = false;
                    rowFilter = "DIAMARCADO is not null and QUANDO = '" + ListaDeDatas.Text + "'";
                    TarefasBindingSource.Filter = rowFilter;
                    break;

                    
            }
            
            MSGtoolStripStatusLabel.Text = "Aplicado um filtro de data";
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            Form calculator1 = new CalculatorForm();
            calculator1.Show();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            switch (toolStripStatusLabel1.Tag.ToString())
            {
                case "Apagado":
                    toolStripStatusLabel1.Image = global::Vida.Properties.Resources.LIGHTON;
                    toolStripStatusLabel1.Tag = "Aceso";
                    this.DataGridView1.BackgroundColor = Color.White;
                    this.DataGridView1.DefaultCellStyle.BackColor = Color.White;
                    this.DataGridView1.ForeColor = Color.Black;
                    break;

                case "Aceso":
                    toolStripStatusLabel1.Image = global::Vida.Properties.Resources.LIGHTOFF;
                    this.DataGridView1.DefaultCellStyle.BackColor = Color.FromArgb(50, 10, 100);
                    this.DataGridView1.ForeColor = Color.White;
                    toolStripStatusLabel1.Tag = "Apagado";
                    break;
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            //tableLayoutPanel6.Enabled = true;

            // Desbloquear elementos
            OQuePretendido.Enabled = true; QuandoPrevisto.Enabled = true;
            ComboBoxPorque.Enabled = true; flowLayoutPanel4.Enabled = true;
            ButtonAnexa.Enabled = true; bindingNavigatorDeleteItem.Enabled = true;
            ButtonAnexa.Enabled = true; BindingExclui.Enabled = true;
            PictureBoxEditar.Enabled = true;
            //BindingNavigatorNovo.Enabled = true;
            
            
            label1.Text = "Prompt".ToUpper();   Old_label = label1.Text;
            toolStripButton16.Visible = false;

            // libera a função de inserir
            dataHoje = DateTime.Today;
            dataPara = Convert.ToDateTime(ListaDeDatas.SelectedItem);
            if (dataPara == dataHoje)
            {
                MSGtoolStripStatusLabel.Text =
                    "Arquivo de dados: " + BancoDados;

                if (bindingNavigatorAddNewItem.Enabled == false)
                { bindingNavigatorAddNewItem.Enabled = true; }
                if (bindingNavigatorDeleteItem.Enabled == false) { bindingNavigatorDeleteItem.Enabled = true; }
                if (bindingNavigatorAddNewItem.Text != "&Inserir")
                { bindingNavigatorAddNewItem.Text = "&Inserir"; }
            }

        }

        private void ButtonAnexa_Click(object sender, EventArgs e)
        {
            try
            {
                TarefasBindingSource.EndEdit();
                TarefasDataSet.AcceptChanges();

                //
                if (TarefasDataSet == null) { return; }

                // Create a file name to write to.
                string filename = VariáveisGlobais.CaminhoBancoDeDados;

                // Create the FileStream to write with.
                System.IO.FileStream stream = new System.IO.FileStream
                    (filename, System.IO.FileMode.Create);

                // Create an XmlTextWriter with the fileStream.
                System.Xml.XmlTextWriter xmlWriter =
                    new System.Xml.XmlTextWriter(stream,
                    System.Text.Encoding.Unicode);

                // Write to the file with the WriteXml method.
                TarefasDataSet.WriteXml(xmlWriter);
                xmlWriter.Close();
                MSGtoolStripStatusLabel.Text = "Anexada a tarefa no arquivo XML " +
                    DateTime.Now.ToString();

                this.PictureBoxEditar.Image = global::Vida.Properties.Resources.Edit1;
                label1.Text = "Prompt".ToUpper(); Old_label = label1.Text;

                if (ContadorDeClique > 0)
                {
                    ContadorDeClique = 0;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void pastaDoItemToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                     return; 
                }
                string Caminho = VariáveisGlobais.MyPath + "\\" + textBox1.Text;
                if (string.IsNullOrEmpty(PastaOculto.Text))
                {

                // Cria a pasta não existente
                    if (!Directory.Exists(Caminho))
                    { 
                        // prevista uma mensagem de confirmação
                        Directory.CreateDirectory(Caminho); PastaOculto.Text = textBox1.Text;
                        goto Pasta; 
                    }

                }
                
                Caminho = VariáveisGlobais.MyPath + "\\" + PastaOculto.Text;
                Pasta:
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Caminho,
                    UseShellExecute = true
                });

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Erro",MessageBoxButtons.OK);
            }

        }

        private void toolStripButtonImpressos_Click(object sender, EventArgs e)
        {
            // Abre uma pasta no explorer de arquivos
            Process.Start(new ProcessStartInfo() {  FileName = VariáveisGlobais.CaminhoDosImpressos, UseShellExecute = true });
        }

        private void checkBoxExibirPrévia_Click(object sender, EventArgs e)
        {
            if(checkBoxExibirPrévia.Checked == false )
            {
                splitContainer7.Panel1Collapsed = true;
                checkBoxExibirPrévia.Text = "Prévia desligado";
            }
            else
            {
                splitContainer7.Panel1Collapsed = false;
                checkBoxExibirPrévia.Text = "Prévia ligado";
                FiltraDadosPrévia();
                
            }
            ;
        }

        private void ferramentasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ferramentasToolStripMenuItem.Checked == false) 
            { 
                tableLayoutPanel5.Visible = false;
                ferramentasToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else 
            {
                tableLayoutPanel5.Visible = true;
                ferramentasToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void atualizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FiltraDadosMarcados();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataGridView MinhaGrade = DataGridView1 as DataGridView; 
            DataGridViewRow MinhaLinha = DataGridView1.CurrentRow;
            DataGridViewCell MinhaCélula = MinhaLinha.Cells[0];
            if (MinhaGrade == null) { return; }

            int i = MinhaCélula.RowIndex;
            if (MinhaCélula.ColumnIndex == 0)
            {
                if (Convert.ToBoolean(DataGridView1.Rows[i].Cells[8].Value) == false)
                {
                    DataGridView1.Rows[i].Cells[8].Value = true;

                }
                // Migrar a linha atual
                DataRow Row;
                Row = BomDiaTarefas.NewRow();
                Row["QUANDO"] = DateTime.Today.ToShortDateString();
                Row["OQUE"] = DataGridView1.Rows[i].Cells[1].Value; //4ª col
                Row["PORQUE"] = DataGridView1.Rows[i].Cells[4].Value; //5ª col


                Row["PESO"] = DataGridView1.Rows[i].Cells[5].Value; //6ª col
                Row["CRITÉRIO"] = DataGridView1.Rows[i].Cells[6].Value; //7ª col
                Row["DIAMARCADO"] = DataGridView1.Rows[i].Cells[3].Value; //3ª col
                Row["Pasta"] = DataGridView1.Rows[i].Cells[9].Value;
                Row["User"] = Responsável.Text;
                toolStripStatusLabelUsuário.Text = Responsável.Text;
                // Índice
                BomDiaTarefas.Rows.Add(Row);
                ContadorDeClique += 1;

                string MsgTexto =
                    DataGridView1.Rows[i].Cells[0].Value.ToString();

                label1.Text = Old_label + "(" + ContadorDeClique + ")";
                MsgTexto = "Migrado IND(" + MsgTexto + ") para o tempo real.";
                MSGtoolStripStatusLabel.Text = "Ok. " + MsgTexto;
                // Desmarcar linha
                DataGridView1.Rows[i].Cells[8].Value = false;
            }



        }

        private void abrirOPadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirPad();
        }
    }
}
