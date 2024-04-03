using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace DapProgram
{
    public partial class Form1 : Form
    {

        private static string dbCommand = "";
        private static BindingSource bindingSrc;

        private static string dbPath = Application.StartupPath + "\\" + "VehicleDB.db;";
        private static string conString = "Data Source=" + dbPath + "Version=3;New=False;Compress=True;";

        private static SQLiteConnection connection = new SQLiteConnection(conString);
        private static SQLiteCommand command = new SQLiteCommand("", connection);

        private static string sql;
        public Form1()
        {
            InitializeComponent();
            this.txtVehicleID.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            openConnection();

            updateDataBiding();

            closeConnection();
        }

        private void closeConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
               // MessageBox.Show("La conexión es:" + connection.State.ToString());
            }
        }

        private void openConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
               // MessageBox.Show("La conexión es:" + connection.State.ToString());
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void displayPosition()
        {
            positionLabel.Text= "Position: " + Convert.ToString(bindingSrc.Position + 1) + "/" + bindingSrc.Count.ToString();
        }

        private void updateDataBiding(SQLiteCommand cmd = null)
        {

            try
            {
                TextBox tb;
                foreach (Control c in groupBox1.Controls)
                {
                    if (c.GetType() == typeof(TextBox))
                    {
                        tb = (TextBox)c;
                        tb.DataBindings.Clear();
                        tb.Text = "";
                    }
                }

                dbCommand = "SELECT";

                sql = "SELECT * FROM Vehicle ORDER BY Vehicle_Id ASC;";

                if (cmd == null)
                {
                    command.CommandText = sql;
                }
                else
                {
                    command = cmd;
                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataSet dataSt = new DataSet();
                adapter.Fill(dataSt, "Vehicle");

                bindingSrc = new BindingSource();
                bindingSrc.DataSource = dataSt.Tables["Vehicle"];

                txtVehicleID.DataBindings.Add("Text", bindingSrc, "Vehicle_Id");
                txtVehicleName.DataBindings.Add("Text", bindingSrc, "Vehicle_Name");
                txtVehicleBrand.DataBindings.Add("Text", bindingSrc, "Vehicle_Brand");
                txtVehicleType.DataBindings.Add("Text", bindingSrc, "Vehicle_Type");
                txtModel.DataBindings.Add("Text", bindingSrc, "Model");
                txtVersion.DataBindings.Add("Text", bindingSrc, "Version");
                txtAnio.DataBindings.Add("Text", bindingSrc, "Anio");
                txtProductName.DataBindings.Add("Text", bindingSrc, "Product");
                txtProductBrand.DataBindings.Add("Text", bindingSrc, "Product_Brand");
                txtProductCode.DataBindings.Add("Text", bindingSrc, "Product_Code");
                txtSystemType.DataBindings.Add("Text", bindingSrc, "System_Type");

                dataGridView1.Enabled = true;
                dataGridView1.DataSource = bindingSrc;

                dataGridView1.AutoResizeColumns((DataGridViewAutoSizeColumnsMode)DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                dataGridView1.Columns[0].Width = 70;

                displayPosition();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Data Binding Error:" + ex.Message.ToString(),
                    "Error Message: DAP",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void moveFirstButton_Click(object sender, EventArgs e)
        {
            bindingSrc.MoveFirst();
            displayPosition();
        }

        private void movePreviousButton_Click(object sender, EventArgs e)
        {
            bindingSrc.MovePrevious();
            displayPosition();
        }

        private void moveNextButton_Click(object sender, EventArgs e)
        {
            bindingSrc.MoveNext();
            displayPosition();
        }

        private void moveLastButton_Click(object sender, EventArgs e)
        {
            bindingSrc.MoveLast();
            displayPosition();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                displayPosition();
            }
            catch(Exception)
            {

            }
        }

        private void refreshDataButton_Click(object sender, EventArgs e)
        {
            if (addNewButton.Text.Equals("Cancelar"))
            {
                return;
            }

            updateDataBiding();
        }

        private void addNewButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(addNewButton.Text == "Agregar")
                {
                    addNewButton.Text = "Cancelar";
                    positionLabel.Text = "Position: 0/0";
                    dataGridView1.ClearSelection();
                    dataGridView1.Enabled = false;
                }
                else
                {
                    addNewButton.Text = "Agregar";
                    updateDataBiding();
                    return;
                }

                TextBox txt;
                foreach(Control c in groupBox1.Controls)
                {
                    if(c.GetType()== typeof(TextBox))
                    {
                        txt = (TextBox)c;
                        txt.DataBindings.Clear();
                        txt.Text = "";
                        if (txt.Name.Equals("txtVehicleName"))
                        {
                            if (txt.CanFocus)
                            {
                                txt.Focus();
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {

            }
        }

        private void addCmdParameters()
        {
            command.Parameters.Clear();
            command.CommandText = sql;

            command.Parameters.AddWithValue("Vehicle_Name", txtVehicleName.Text.Trim());
            command.Parameters.AddWithValue("Vehicle_Brand", txtVehicleBrand.Text.Trim());
            command.Parameters.AddWithValue("Vehicle_Type", txtVehicleType.Text.Trim());
            command.Parameters.AddWithValue("Model", txtModel.Text.Trim());
            command.Parameters.AddWithValue("Version", txtVersion.Text.Trim());
            command.Parameters.AddWithValue("Anio", txtAnio.Text.Trim());
            command.Parameters.AddWithValue("Product", txtProductName.Text.Trim());
            command.Parameters.AddWithValue("Product_Brand", txtProductBrand.Text.Trim());
            command.Parameters.AddWithValue("Product_Code", txtProductCode.Text.Trim());
            command.Parameters.AddWithValue("System_Type", txtSystemType.Text.Trim());

            if(dbCommand.ToUpper()== "UPDATE")
            {
                command.Parameters.AddWithValue("Vehicle_Id", txtVehicleID.Text.Trim());
            }

        }

        private void saveButton_Click(object sender, EventArgs e)
        {

            if(string.IsNullOrEmpty(txtVehicleName.Text.Trim()) || 
                    string.IsNullOrEmpty(txtVehicleBrand.Text.Trim()) ||
                    string.IsNullOrEmpty(txtVehicleType.Text.Trim()) ||
                    string.IsNullOrEmpty(txtModel.Text.Trim()) ||
                    string.IsNullOrEmpty(txtVersion.Text.Trim()) ||
                    string.IsNullOrEmpty(txtAnio.Text.Trim()) ||
                    string.IsNullOrEmpty(txtProductName.Text.Trim()) ||
                    string.IsNullOrEmpty(txtProductBrand.Text.Trim()) ||
                    string.IsNullOrEmpty(txtProductCode.Text.Trim()) ||
                    string.IsNullOrEmpty(txtSystemType.Text.Trim()))
            {

                MessageBox.Show("Please fill in the required fields.", "Add new Record", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;

            }

            openConnection();

            try
            {
                if (addNewButton.Text == "Agregar")
                {
                    if (txtVehicleID.Text.Trim() == "" || string.IsNullOrEmpty(txtVehicleID.Text.Trim()))
                    {
                        MessageBox.Show("Please select an item");
                        return;
                    }

                    if (MessageBox.Show("ID: " + txtVehicleID.Text.Trim() +
                        "¿Quieres actualizar el siguiente vehiculo?",
                        "Actualizar",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        return;
                    }

                    dbCommand = "UPDATE";

                    sql = "UPDATE Vehicle SET Vehicle_Name = @Vehicle_Name, Vehicle_Brand = @Vehicle_Brand, Vehicle_Type = @Vehicle_Type, Model = @Model, Version = @Version, Anio = @Anio, Product = @Product, Product_Brand = @Product_Brand, Product_Code = @Product_Code, System_Type = @System_Type WHERE Vehicle_Id = @Vehicle_Id";

                    addCmdParameters();

                }
                else if (addNewButton.Text.Equals("Cancelar"))
                {
                    DialogResult result;
                    result = MessageBox.Show("¿Quieres agregar un nuevo vehículo? (Y/N)",
                        "Agregar Vehículo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if(result == DialogResult.Yes)
                    {
                        dbCommand = "INSERT";
                        sql= "INSERT INTO Vehicle(Vehicle_Name, Vehicle_Brand, Vehicle_Type, Model, Version, Anio, Product, Product_Brand, Product_Code, System_Type)" +
                            "VALUES(@Vehicle_Name, @Vehicle_Brand, @Vehicle_Type, @Model, @Version, @Anio, @Product, @Product_Brand, @Product_Code, @System_Type)";


                        addCmdParameters();
                    }
                    else
                    {
                        return;
                    }
                }

                int executeResult = command.ExecuteNonQuery();

                if(executeResult == -1)
                {
                    MessageBox.Show("Data was not saved", "Fail to save data", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }
                else
                {
                    MessageBox.Show("Your SQL" + dbCommand + "QUERY has been executed successfully",
                        "Guardado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    updateDataBiding();

                    addNewButton.Text = "Agregar";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbCommand = "";
                closeConnection();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (addNewButton.Text == "Cancek")
            {
                return;
            }

            if(txtVehicleID.Text.Trim()==""||
                string.IsNullOrEmpty(txtVehicleID.Text.Trim()))
            {
                MessageBox.Show("Selecciona un item de la Lista",
                    "Borra Vehículo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            openConnection();

            try
            {
                if(MessageBox.Show("ID: " + txtVehicleID.Text.Trim() +
                    "¿Quieres borrar este vehículo?",
                    "Borra Vehículo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                dbCommand = "DELETE";

                sql = "DELETE FROM Vehicle WHERE Vehicle_Id = @Vehicle_Id";
                command.Parameters.Clear();
                command.CommandText = sql;
                command.Parameters.AddWithValue("Vehicle_Id", txtVehicleID.Text.Trim());

                int executeResult = command.ExecuteNonQuery();
                if(executeResult == 1)
                {
                    MessageBox.Show("YourSQL" + dbCommand + "QUERY has been executed successfully",
                        "Delete Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    updateDataBiding();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error Message",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbCommand = "";
                closeConnection();
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if(addNewButton.Text == "Cancelar")
            {
                return;
            }

            openConnection();

            try
            {
                if (string.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    updateDataBiding();
                    return;
                }

                sql = "SELECT * FROM Vehicle ";
                sql += "WHERE Vehicle_Name LIKE @Keyword2 ";
                sql += "OR Vehicle_Brand LIKE @Keyword2 ";
                sql += "OR Vehicle_Type LIKE @Keyword2 ";
                sql += "OR Model LIKE @Keyword2 ";
                sql += "OR Version LIKE @Keyword2 ";
                sql += "OR Anio LIKE @Keyword2 ";
                sql += "OR Product LIKE @Keyword2 ";
                sql += "OR Product_Brand LIKE @Keyword2 ";
                sql += "OR Product_Code LIKE @Keyword2 ";
                sql += "OR System_Type LIKE @Keyword2 ";
                sql += "ORDER BY Vehicle_id ASC";


                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                command.Parameters.Clear();

                string keyword = string.Format("%{0}%", txtKeyword.Text);

                command.Parameters.AddWithValue("Keyword2", keyword);

                updateDataBiding(command);

            } 
            catch (Exception ex)
            {
                MessageBox.Show("Search Error: " + ex.Message.ToString(),
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                closeConnection();
                txtKeyword.Focus();
            }
        }

 
    }
}
