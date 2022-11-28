﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace WMS.DataAccess.Repository
{
    public class TenantWarehouseRepository : Repository<MasDataTenantWarehouse>, ITenantWarehouseRepository
    {
        private readonly AppDbContext db;

        public TenantWarehouseRepository(AppDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(MasDataTenantWarehouse masDataTenantWarehouse)
        {
           db.Update(masDataTenantWarehouse);
        }
    }
}
