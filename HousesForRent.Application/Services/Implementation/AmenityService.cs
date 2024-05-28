using HousesForRent.Application.Common.Interfaces;
using HousesForRent.Application.Services.Interface;
using HousesForRent.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HousesForRent.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AmenityService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;
        }
        public void CreateAmenity(Amenity amenity)
        {
            _unitOfWork.Amenity.Add(amenity);
            _unitOfWork.Amenity.Save();
        }

        public bool DeleteAmenity(int id)
        {
            try
            {
                Amenity? amenity = _unitOfWork.Amenity.Get(u => u.Id == id);

                if (amenity is not null)
                {
                    _unitOfWork.Amenity.Remove(amenity);
                    _unitOfWork.Amenity.Save();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Amenity GetAmenity(int id)
        {
            return _unitOfWork.Amenity.Get(u=>u.Id == id);    
        }

        public IEnumerable<Amenity> GetAllAmenities()
        {
            return  _unitOfWork.Amenity.GetAll();
        }

        public void UpdateAmenity(Amenity amenity)
        {
            _unitOfWork.Amenity.Update(amenity);
            _unitOfWork.Amenity.Save();
        }
    }
}
