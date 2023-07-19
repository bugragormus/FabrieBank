﻿using System;
using System.Data.SQLite;
using FabrieBank.DAL;
using Microsoft.Data.SqlClient;

namespace FabrieBank.Entity
{
    public class CreateAccountDB
    {
        private DataAccessLayer dataAccessLayer;

        public CreateAccountDB()
        {
            dataAccessLayer = new DataAccessLayer();
        }

        public void CreateAccount(int musteriId, int dovizCinsi, string hesapAdi)
        {
            dataAccessLayer.CreateAccount(musteriId, dovizCinsi, hesapAdi);
        }

        public string GetAndIncrementHesapNumarasi(int dovizCinsi, SqlConnection connection)
        {
            return dataAccessLayer.GetAndIncrementHesapNumarasi(dovizCinsi, connection);
        }
    }
}