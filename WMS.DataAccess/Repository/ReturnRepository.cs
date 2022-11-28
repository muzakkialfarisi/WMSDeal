﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class ReturnRepository : Repository<InvReturn>, IReturnRepository
    {
        private readonly AppDbContext db;

        public ReturnRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(InvReturn invPickingRoute)
        {
           db.Update(invPickingRoute);
        }
    }
}
