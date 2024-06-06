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
    public class HouseService : IHouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HouseService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviroment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviroment = webHostEnviroment;
        }
        public void CreateHouse(House house)
        {
            if (house.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(house.Image.FileName);
                string imagePath = Path.Combine(_webHostEnviroment.WebRootPath, @"images\House");

                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    house.Image.CopyTo(fileStream);

                house.ImageUrl = @"\images\House\" + fileName;
            }

            house.Occupancy = house.SingleBedQuantity + house.DoubleBedQuantity * 2;

            _unitOfWork.House.Add(house);
            _unitOfWork.House.Save();

        }

        public bool DeleteHouse(int id)
        {
            try
            {
                House? house = _unitOfWork.House.Get(u => u.Id == id);

                if (house is not null)
                {
                    if (!string.IsNullOrEmpty(house.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath, house.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    _unitOfWork.House.Remove(house);
                    _unitOfWork.House.Save();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public House GetHouse(int id)
        {
            return _unitOfWork.House.Get(u => u.Id == id);
        }

        public IEnumerable<House> GetAllHouses(int? peopleQty = null)
        {
            if (peopleQty.HasValue)
            {
                return _unitOfWork.House.GetAll(u => u.Occupancy >= peopleQty);

            }
            else
                return _unitOfWork.House.GetAll();
        }

        public void UpdateHouse(House house)
        {
            if (house.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(house.Image.FileName);
                string imagePath = Path.Combine(_webHostEnviroment.WebRootPath, @"images\House");

                if (!string.IsNullOrEmpty(house.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnviroment.WebRootPath, house.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    house.Image.CopyTo(fileStream);

                house.ImageUrl = @"\images\House\" + fileName;
            }

            _unitOfWork.House.Update(house);
            _unitOfWork.House.Save();
        }
    }
}
