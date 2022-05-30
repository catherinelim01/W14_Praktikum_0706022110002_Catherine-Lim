using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace W14_Praktikum_Catherine_Lim_0706022110002
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MySqlConnection sqlConnect = new MySqlConnection("server=localhost;uid=root;pwd=;database=premier_league");
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlAdapter;
        String sqlQuery;
        DataTable dataTeam = new DataTable();
        DataTable dataTopScorer = new DataTable();
        DataTable dataWorstDiscipline = new DataTable();
        DataTable dataLastMatch = new DataTable();
        int PosisiSekarang = 0;
        string simpanKodeTeam;

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlQuery = "select t.team_name, concat(m.manager_name, '(', n.nation, ')'), concat(t.home_stadium, t.city, '(', t.capacity,')'), t.team_id from team t left join manager m on t.manager_id = m.manager_id left join nationality n on m.nationality_id = n.nationality_id group by 1 order by 1;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataTeam);

            IsiDataTeam(0);

        }

        public void IsiDataTeam(int Posisi)
        {
            labelTeamName.Text = dataTeam.Rows[Posisi][0].ToString();
            labelManager.Text = dataTeam.Rows[Posisi][1].ToString();
            labelStadium.Text = dataTeam.Rows[Posisi][2].ToString();
            simpanKodeTeam = dataTeam.Rows[Posisi][3].ToString();

            dataTopScorer = new DataTable();
            dataWorstDiscipline = new DataTable();
            dataLastMatch = new DataTable();

            sqlQuery = "select p.player_name as 'Nama Pemain', sum(d.type = 'GO') + sum(d.type = 'GP') as 'Jumlah Goal', concat(' (', convert(sum(d.type = 'GP'), char), ')') as 'Goal Penalty' from player p, dmatch d where p.player_id = d.player_id and p.team_id = '" + simpanKodeTeam + "' group by 1 having sum(d.type = 'GO') + sum(d.type = 'GP') != 0 order by 2 desc limit 1;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataTopScorer);

            sqlQuery = "select p.player_name, sum(if(d.type = 'CY', 1, 0)) as 'Yellow', sum(if (d.type = 'CR', 1,0)) as 'Red', sum(if(d.type = 'CY',1,0)) + sum(if(d.type = 'CR',3,0)) as 'Poin' from player p, dmatch d where p.player_id = d.player_id and d.team_id = '" + simpanKodeTeam + "' group by p.player_id order by `Poin` desc limit 1;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataWorstDiscipline);

            sqlQuery = "select m.match_date ,date_format(m.match_date, \'%d/%c/%Y') as 'match date', 'HOME' as 'Home/Away', concat('vs ',t.team_name) as 'Lawan', concat(goal_home, ' - ', goal_away) as 'Score' from `match` m, team t where team_home = '" + simpanKodeTeam + "' and m.team_away = t.team_id union select m.match_date ,date_format(m.match_date, \'%d/%c/%Y') as 'match date', 'AWAY' as 'Home/Away', concat('vs ',t.team_name) as 'Lawan', concat(goal_home, ' - ', goal_away) as 'Score' from `match` m, team t where team_away = '" + simpanKodeTeam + "' and m.team_away = t.team_id order by 1 desc limit 5;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dataLastMatch);
            dataGridViewLastMatch.DataSource = dataLastMatch;
            dataGridViewLastMatch.Columns.Remove("match_date");

            labelTopScorer.Text = dataTopScorer.Rows[0][0].ToString() +" "+ dataTopScorer.Rows[0][1].ToString() + dataTopScorer.Rows[0][2].ToString();
            labelWorstDiscipline.Text = dataWorstDiscipline.Rows[0][0].ToString() +"," + dataWorstDiscipline.Rows[0][1].ToString() + " Yellow Card and " + dataWorstDiscipline.Rows[0][2].ToString() + " Red Card";

            PosisiSekarang = Posisi;
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            IsiDataTeam(0);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (PosisiSekarang > 0)
            {
                PosisiSekarang--;
                IsiDataTeam(PosisiSekarang);
            }
            else
            {
                MessageBox.Show("Data Sudah Data Pertama");
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (PosisiSekarang < dataTeam.Rows.Count - 1)
            {
                PosisiSekarang++;
                IsiDataTeam(PosisiSekarang);
            }
            else
            {
                MessageBox.Show("Data Sudah Data Terakhir");
            }
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            IsiDataTeam(dataTeam.Rows.Count - 1);
        }

       
    }
}
