using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using addToTasty.TastyDataSetTableAdapters;

namespace addToTasty
{
    public partial class addRecipe : Form
    {
        TastyDataSet tastyDataSet = new TastyDataSet();
        SqlConnection con;

        RecipesTableAdapter daRecipes = new RecipesTableAdapter();
        Recipe_IngredientTableAdapter daRecipe_Ingredient = new Recipe_IngredientTableAdapter();
        IngredientTableAdapter daIngredient = new IngredientTableAdapter();
        CategoriesTableAdapter daCategories = new CategoriesTableAdapter();

        SqlDataAdapter da;
        TastyDataSet.Recipe_IngredientDataTable table = new TastyDataSet.Recipe_IngredientDataTable();

        QueriesTableAdapter q = new QueriesTableAdapter();

        SqlCommand sqlCommand;
        int? code = 0;
        int recipeCode = 0;

        AutoCompleteStringCollection ingredientList = new AutoCompleteStringCollection();

        List<string> categories;
        List<string> ingredients;

        public addRecipe()
        {
            InitializeComponent();
            

            string conToTasty = ConfigurationManager.ConnectionStrings["myCon"].ConnectionString;
            con = new SqlConnection(conToTasty);
            sqlCommand = new SqlCommand("", con);

            daRecipes.Fill(tastyDataSet.Recipes);
            daIngredient.Fill(tastyDataSet.Ingredient);
            daRecipe_Ingredient.Fill(tastyDataSet.Recipe_Ingredient);

            dataGridView2.DataSource = tastyDataSet.Recipes;

            //adding categories names into the choose category combobox
            sqlCommand.CommandText = "SELECT categoryName FROM categories";
            categories = addToCollection(sqlCommand);

            //ingerdient autoComplete list
            sqlCommand.CommandText = "SELECT ingredientName FROM ingredient";
            ingredients=addToCollection(sqlCommand);

            int i;
            for ( i= 0; i < categories.Count; i++)
            {
                comboBox2.Items.Add(categories[i]);
                ingredientList.Add(ingredients[i]);
            }

            for ( ; i < ingredients.Count; i++)
            {
                ingredientList.Add(ingredients[i]);
            }

            textBox3.AutoCompleteCustomSource = ingredientList;
            textBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || textBox4.Text == "" || textBox5.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("One or more of the fields are empty", "Error");
                return;
            }

            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            textBox3.Visible = true;
            textBox6.Visible = true;
            button3.Visible = true;
            button2.Visible = true;
            dataGridView1.Visible = true;

            textBox2.Enabled = false;
            textBox5.Enabled = false;
            textBox4.Enabled = false;
            comboBox2.Enabled = false;
            button1.Visible = false;

            //specific recipe ingredients table
            q.getRecipeCodeByName(textBox2.Text, ref code);
            recipeCode = code.GetValueOrDefault();
            sqlCommand.CommandText = "select * from Recipe_Ingredient where recipeCode= " + recipeCode;
            da = new SqlDataAdapter(sqlCommand.CommandText, con);
            da.Fill(table);
            dataGridView1.DataSource = table;

            //the recipe name
            label1.Text = textBox2.Text;

            //add the recipe
            q.getCategoryCodeByName(comboBox2.Text, ref code);
            daRecipes.Insert(code, textBox4.Text, textBox5.Text, textBox2.Text);
            daRecipes.Fill(tastyDataSet.Recipes);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("One or more of the fields are empty", "Error");
                return;
            }

            //new ingredient
            int? exsists = 1;
            q.ingredientExists(textBox3.Text, ref exsists);
            if (exsists == 0)
            {
                daIngredient.Insert(textBox3.Text);
                daIngredient.Fill(tastyDataSet.Ingredient);

                //update the autoCompleteList
                sqlCommand.CommandText = "SELECT ingredientName FROM ingredient";
                ingredientList.Add(textBox3.Text);
            }

            //add the ingredient to the recipe
            q.getRecipeCodeByName(label1.Text, ref code);
            recipeCode = code.GetValueOrDefault();
            q.getIngredientCodeByName(textBox3.Text, ref code);
            daRecipe_Ingredient.Insert(code.GetValueOrDefault(), textBox6.Text, recipeCode);
            daRecipe_Ingredient.Fill(tastyDataSet.Recipe_Ingredient);

            //cleaning
            textBox3.Text = "";
            textBox6.Text = "";

            //the ingredients of the specific recipe
            sqlCommand.CommandText = "select * from Recipe_Ingredient where recipeCode= " + recipeCode;
            da = new SqlDataAdapter(sqlCommand.CommandText, con);
            da.Fill(table);
            dataGridView1.DataSource = table;
        }

        private void textBox6_MouseClick(object sender, MouseEventArgs e)
        {
            textBox6.Text = "";
            textBox6.ForeColor = Color.Black;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox3.Visible = false;
            textBox6.Visible = false;
            button3.Visible = false;
            button2.Visible = false;
            dataGridView1.Visible = false;

            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = @"C:\racheli\TastyDatabase\pictures\---name-- -.JPG";
            textBox6.Text = "";
            comboBox2.Text = "";

            dataGridView1.DataSource = null;

            textBox2.Enabled = true;
            textBox5.Enabled = true;
            textBox4.Enabled = true;
            comboBox2.Enabled = true;
            button1.Visible = true;
        }

        //
        //functions
        //

        List<string> addToCollection(SqlCommand command)
        {
            con.Open();
            List<string> list = new List<string>();
            string name = "";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                name = reader.GetString(0);
                list.Add(name);
            }
            con.Close();
            return list;
        }

          private void addRecipe_Load(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void addRecipe_Load_1(object sender, EventArgs e)
        {

        }
    }
}
