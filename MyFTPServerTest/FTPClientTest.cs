using MyFTPServer.MyDBContext;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace MyFTPServerTest
{
    public class FTPClientTest
    {
        //[Fact]
        public void Serialize()
        {
            //using(SqlConnection con = new SqlConnection())
            //{
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        con.Open();
            //        cmd.Connection = con;
            //        cmd.CommandText = "INSERT INTO TABLE() VALUE ";
            //        int v = cmd.ExecuteNonQuery();

            //    }
            //}

            FileStream file = new FileStream(@"C:\Projects\pic.jpg", FileMode.Open);
            byte[] bytes = new byte[file.Length];
            file.Read(bytes, 0, bytes.Length);
            string base64 = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            file.Close();

            List<PhysicalCard> physicalCards = new List<PhysicalCard>();
            for (int i = 0; i < 300000 * 3; i++)
            {
                PhysicalCard physicalCard = new PhysicalCard();
                physicalCard.Name = "WZY" + i.ToString().PadLeft(8);
                physicalCard.Age = i % 60;
                physicalCard.Sex = i % 2 == 0 ? "男" : "女";
                physicalCard.Card = "123456789" + (i).ToString().PadLeft(9);
                physicalCard.Pic = base64;
                physicalCards.Add(physicalCard);
            }

            FileStream file2 = new FileStream(@"D:\ftp_test\testData.helloworld", FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file2, physicalCards);
            file2.Close();
        }

        //[Fact]
        public void Deserialize()
        {
            FileStream file2 = new FileStream(@"D:\ftp_test\testData.helloworld", FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            var data = binaryFormatter.Deserialize(file2) as List<PhysicalCard>;
            file2.Close();
        }


    }
}
