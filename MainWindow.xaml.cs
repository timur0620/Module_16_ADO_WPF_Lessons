using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace Module_16_ADO_WPF_Lessons
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        SqlConnection connecrion;
        SqlDataAdapter adapter;
        DataTable dt;
        DataRowView row;

        public MainWindow()
        {
            InitializeComponent();
            Preparing();
            
        }
        private void Preparing()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder() 
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = @"MyFirstSQL_2",
                IntegratedSecurity = true,
                Pooling = false,
                //UserID = "Admin",
                //Password = "Admin"

            };

            adapter = new SqlDataAdapter();
            connecrion = new SqlConnection(connectionStringBuilder.ConnectionString);
            dt = new DataTable();

            var sql = @"Select * from Workers order by Workers.id";
            adapter.SelectCommand = new SqlCommand(sql, connecrion); // select

            sql = @"insert into Workers (WorkerName, IdBoss, IdDescriptions) 
                                 VALUES (@WorkerName, @IdBoss, @IdDescriptions);
                                 SET @Id = @@IDENTITY;";

            adapter.InsertCommand = new SqlCommand(sql,connecrion);

            adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
            adapter.InsertCommand.Parameters.Add("@WorkerName", SqlDbType.Int, 20, "WorkerName");
            adapter.InsertCommand.Parameters.Add("@IdBoss", SqlDbType.Int, 4, "IdBoss");
            adapter.InsertCommand.Parameters.Add("@IdDescriptions", SqlDbType.Int, 4, "Id");


            sql = @"Update  Workers Set 
                            WorkerName = @WorkerName,
                            IdBoss = @IdBoss,
                            IdDescriptions = @IdDescriptions
                   Where Id = @Id";

            adapter.UpdateCommand = new SqlCommand(sql, connecrion);

            adapter.UpdateCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id").SourceVersion = DataRowVersion.Original;
            adapter.UpdateCommand.Parameters.Add("@WorkerName", SqlDbType.Int, 20, "WorkerName");
            adapter.UpdateCommand.Parameters.Add("@IdBoss", SqlDbType.Int, 4, "IdBoss");
            adapter.UpdateCommand.Parameters.Add("@IdDescriptions", SqlDbType.Int, 4, "Id");

            sql = "Delete from Workers Where Id = @Id";
            adapter.DeleteCommand = new SqlCommand(sql, connecrion);
            adapter.DeleteCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id");

            adapter.Fill(dt);
            gridView.DataContext = dt.DefaultView;


        }
            
        private void gridView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (row == null) return;
            row.EndEdit();
            adapter.Update(dt);
        }

        private void gridView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            row = (DataRowView)gridView.SelectedItem;
            row.BeginEdit();
            adapter.Update(dt);
        }
        private void AllViewShow(object sender, RoutedEventArgs e)
        {
            new AllView().ShowDialog();
        }
        private void MenuItemAddClick(object sender, RoutedEventArgs e)
        {
            DataRow r = dt.NewRow();
            AddWindow add = new AddWindow(r);
            add.ShowDialog();

            if (add.DialogResult.Value)
            {
                dt.Rows.Add(r);
                adapter.Update(dt);
            }
        }
        private void MenuItemDeleteClick(object sender, RoutedEventArgs e)
        {
            row = (DataRowView)gridView.SelectedItem;
            row.Row.Delete();
            adapter.Update(dt);
        }
    }
}
