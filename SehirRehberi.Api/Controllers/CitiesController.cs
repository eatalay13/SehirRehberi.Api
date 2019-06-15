using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SehirRehberi.Api.Data;
using SehirRehberi.Api.Dtos;
using SehirRehberi.Api.Models;

namespace SehirRehberi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IAppRepository appRepository;
        private readonly IMapper mapper;

        public CitiesController(IAppRepository appRepository, IMapper mapper)
        {
            this.appRepository = appRepository;
            this.mapper = mapper;
        }

        public ActionResult Get()
        {
            var cities = appRepository.GetCities();
            var citiesToReturn = mapper.Map<List<CityForListDto>>(cities);
            return Ok(citiesToReturn);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var city = appRepository.GetCityById(id);
            var cityToReturn = mapper.Map<CityForDetailDto>(city);
            return Ok(cityToReturn);
        }

        [HttpPost]
        public ActionResult Post([FromBody]City city)
        {
            appRepository.Add(city);
            appRepository.SaveAll();
            return Ok(city);
        }

        [HttpGet("{id}")]
        [Route("photos")]
        public ActionResult GetPhotosByCity(int cityId)
        {
            var photos = appRepository.GetPhotosByCity(cityId);
            return Ok(photos);
        }
    }
}