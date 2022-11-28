﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class HouseCodeRepository : Repository<MasHouseCode>, IHouseCodeRepository
    {
        private readonly AppDbContext db;

        public HouseCodeRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasHouseCode masHouseCode)
        {
            db.Update(masHouseCode);
        }
    }
}