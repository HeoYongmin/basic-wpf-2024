using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex12_Busan_ExhibitionList.Models
{
    internal class Exhibit
    {
        public int Id { get; set; }
        public int Res_no { get; set; }
        public string Title { get; set; }
        public string Op_st_dt { get; set; }
        public string Op_ed_dt { get; set; }
        public string Op_at { get; set; }
        public string Place_Id { get; set; }
        public string Place_nm { get; set; }
        public string Showtime { get; set; }
        public string Price { get; set; }
        public string Crew { get; set; }
        public string Dabom_url { get; set; }

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[Exhibit]
                                                                   ([Res_no]
                                                                   ,[Title]
                                                                   ,[Op_st_dt]
                                                                   ,[Op_ed_dt]
                                                                   ,[Op_at]
                                                                   ,[Place_id]
                                                                   ,[Place_nm]
                                                                   ,[Showtime]
                                                                   ,[Price]
                                                                   ,[Crew]
                                                                   ,[Dabom_url])
                                                             VALUES
                                                                   (@Res_no
                                                                   ,@Title
                                                                   ,@Op_st_dt
                                                                   ,@Op_ed_dt
                                                                   ,@Op_at
                                                                   ,@Place_id
                                                                   ,@Place_nm
                                                                   ,@Showtime
                                                                   ,@Price
                                                                   ,@Crew
                                                                   ,@Dabom_url)";

        public static readonly string SELECT_QUERY = @"SELECT [Id]
                                                              ,[Res_no]
                                                              ,[Title]
                                                              ,[Op_st_dt]
                                                              ,[Op_ed_dt]
                                                              ,[Op_at]
                                                              ,[Place_id]
                                                              ,[Place_nm]
                                                              ,[Showtime]
                                                              ,[Price]
                                                              ,[Crew]
                                                              ,[Dabom_url]
                                                          FROM [dbo].[Exhibit]";

        public static readonly string DELETE_QUERY = @"DELETE FROM [dbo].[Exhibit]
                                                         WHERE Res_no = @Res_no";
    }
}
