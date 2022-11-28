﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class RepackRepository : Repository<InvRepacking>, IRepackRepository
    {
        private readonly AppDbContext db;

        public RepackRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvRepacking inv)
        {
           db.Update(inv);
        }
    }
}
