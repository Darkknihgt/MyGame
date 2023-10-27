using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;



class Program
{
    static MySqlConnection conn = null;
    static void Main(string[] args)
    {
        conn = new MySqlConnection("server=localhost;User Id=root;password=;Database=studymysql;Charset=utf8");
        conn.Open();

        Add();
        //Query();
        //Update();
        //Delete();

        Console.ReadKey();
        conn.Close();
    }

    public static void Add()
    {
        MySqlCommand cmd = new MySqlCommand("insert into userinfo set name='Lina',age=18", conn);
        cmd.ExecuteNonQuery();
        int id = (int)cmd.LastInsertedId;
        Console.WriteLine($"Sql Insert key{id}");



    }

    public static void Delete()
    {
        MySqlCommand cmd = new MySqlCommand("delete from userinfo where id = 3", conn);
        cmd.ExecuteNonQuery();

        Console.WriteLine("delete done");
    }

    public static void Update()
    {
        MySqlCommand cmd = new MySqlCommand("update userinfo set name =@name,age = @age where id=@id", conn);
        cmd.Parameters.AddWithValue("name", "God");
        cmd.Parameters.AddWithValue("age", 100);
        cmd.Parameters.AddWithValue("id", 2);

        cmd.ExecuteNonQuery();
        Console.WriteLine("Update Done");
    }

    public static void Query()
    {
        MySqlCommand cmd = new MySqlCommand("select*from userinfo where name='Luna'", conn);
        MySqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int id = reader.GetInt32("id");
            string name = reader.GetString("name");
            int age = reader.GetInt32("age");

            Console.WriteLine($"sql result : \tid: {id}\tname: {name}\tage: {age}.");
        }
    }
}

