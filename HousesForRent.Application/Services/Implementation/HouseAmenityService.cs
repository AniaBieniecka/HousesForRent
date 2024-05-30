using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Implementation
{
    public class HouseAmenityService : IHouseAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HouseAmenityService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;
        }

        public void CreateHouseAmenity(HouseAmenity houseAmenity)
        {
            _unitOfWork.HouseAmenity.Add(houseAmenity);
            _unitOfWork.HouseAmenity.Save();
        }

        public bool DeleteHouseAmenity(int id)
        {
            try
            {
                HouseAmenity? houseAmenity = _unitOfWork.HouseAmenity.Get(u => u.Id == id);

                if (houseAmenity is not null)
                {
                    _unitOfWork.HouseAmenity.Remove(houseAmenity);
                    _unitOfWork.HouseAmenity.Save();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<HouseAmenity> GetAllHouseAmenities(int? houseId)
        {
            if(houseId is not null)
            {
                return _unitOfWork.HouseAmenity.GetAll(u => u.HouseId == houseId);
            }
            else return _unitOfWork.HouseAmenity.GetAll();

        }

        public HouseAmenity GetHouseAmenity(int houseId, int amenityId)
        {
            return _unitOfWork.HouseAmenity.Get(u => u.HouseId == houseId && u.AmenityId == amenityId);
        }

        public void UpdateHouseAmenity(HouseAmenity houseAmenity)
        {
            _unitOfWork.HouseAmenity.Update(houseAmenity);
            _unitOfWork.HouseAmenity.Save();
        }
    }
}
