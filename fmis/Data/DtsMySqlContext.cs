using fmis.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Data
{
    public class DtsMySqlContext : BaseEntityTimeStramp
    {
        public string ConnectionString { get; set; }

        public DtsMySqlContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Dts> allDts()
        {
            List<Dts> list = new List<Dts>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
    
                MySqlCommand cmd = new MySqlCommand(@"
                    select 
	                    dts.DtsId,
                        dts.Description,
                    from 
	                    dts.Dts dts 
                    order by 
                         asc
                ", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Dts()
                        {
                            Description = reader["Description"].ToString(),
                        });
                    }
                }
            }

            return list;
        }


        public List<Dts> forDts(string dtsId)
        {
            List<Dts> list = new List<Dts>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                
                MySqlCommand cmd = new MySqlCommand(@"
                    select 
	                    dts.DtsId,
                        dts.Description,
                    from 
	                    dts.dts dts 
	                where 
		                dts.dtsId in " + dtsId +
                    @"order by 
                         asc", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Dts()
                        {
                            Description = reader["Description"].ToString(),
                        });
                    }
                }
            }

            return list;
        }

    }
}
