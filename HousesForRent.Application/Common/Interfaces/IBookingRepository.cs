﻿using HousesForRent.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Common.Interfaces
{
    public interface IBookingRepository: IRepository<Booking>
    {
        void Update(Booking booking);
        void Save();

    }
}
