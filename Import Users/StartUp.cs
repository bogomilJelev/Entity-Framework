using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Models.dto;

namespace ProductShop
{

    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            string json = File.ReadAllText("../../../Datasets/users.json");
            var result = ImportUsers(context, json);
            Console.WriteLine(result);

        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            initiautomaper();
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<dtouser>>(inputJson);
            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";

        }

        private static void initiautomaper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            mapper = config.CreateMapper();
        }
    }
}