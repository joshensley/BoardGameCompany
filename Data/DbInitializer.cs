using BoardGameCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameCompany.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.BoardGame.Any())
            {
                return;
            }

            var brand = new Brand[]
            {
                new Brand{BrandName="ABC"},
                new Brand{BrandName="DEF"},
                new Brand{BrandName="GHI"},
                new Brand{BrandName="JKL"},
                new Brand{BrandName="MNO"},
                new Brand{BrandName="PQR"},
                new Brand{BrandName="STU"}
            };

            context.Brands.AddRange(brand);
            context.SaveChanges();

            var boardGame = new BoardGame[]
            {
                new BoardGame{BrandID=1,BoardGameImageFilePath="/images/boardgames/gloomhaven.jpg",UPC="123456789123",Title="Gloomhaven",Description="some text",PlayerNumber=4,ReleaseDate=DateTime.Parse("2019-12-12"),Price=25},
                new BoardGame{BrandID=2,BoardGameImageFilePath="/images/boardgames/pipeline.jpg",UPC="223456789123",Title="Pipeline",Description="some text",PlayerNumber=5,ReleaseDate=DateTime.Parse("2018-12-12"),Price=50},
                new BoardGame{BrandID=3,BoardGameImageFilePath="/images/boardgames/wingspan.jpg",UPC="323456789123",Title="Wingspan",Description="some text",PlayerNumber=5,ReleaseDate=DateTime.Parse("2017-12-12"),Price=40},
                new BoardGame{BrandID=4,BoardGameImageFilePath="/images/boardgames/tammany_hall.jpg",UPC="423456789123",Title="Tammany Hall",Description="some text",PlayerNumber=5,ReleaseDate=DateTime.Parse("2016-12-12"),Price=25},
                new BoardGame{BrandID=5,BoardGameImageFilePath="/images/boardgames/watergate.jpg",UPC="523456789123",Title="Watergate",Description="some text",PlayerNumber=2,ReleaseDate=DateTime.Parse("2020-12-12"),Price=20},
                new BoardGame{BrandID=6,BoardGameImageFilePath="/images/boardgames/a_war_of_whispers.jpg",UPC="623456789123",Title="A War of Whispers",Description="some text",PlayerNumber=4,ReleaseDate=DateTime.Parse("2018-12-12"),Price=50},
                new BoardGame{BrandID=7,BoardGameImageFilePath="/images/boardgames/arkham_horror_tcg.jpg",UPC="723456789123",Title="Arkham Horror: The Card Game",Description="some text",PlayerNumber=2,ReleaseDate=DateTime.Parse("2020-12-12"),Price=30},
                new BoardGame{BrandID=1,BoardGameImageFilePath="/images/boardgames/dune.jpg",UPC="823456789123",Title="Dune",Description="some text",PlayerNumber=6,ReleaseDate=DateTime.Parse("2020-12-12"),Price=50},
                new BoardGame{BrandID=2,BoardGameImageFilePath="/images/boardgames/root.jpg",UPC="923456789123",Title="Root",Description="some text",PlayerNumber=5,ReleaseDate=DateTime.Parse("2016-12-12"),Price=30},
                new BoardGame{BrandID=3,BoardGameImageFilePath="/images/boardgames/gloomhaven_jotl.jpg",UPC="113456789123",Title="Gloomhaven: Jaws of the Lion",Description="Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                ,PlayerNumber=4,ReleaseDate=DateTime.Parse("2020-12-12"),Price=20}
            };

            context.BoardGame.AddRange(boardGame);
            context.SaveChanges();

            var inventoryItem = new InventoryItem[]
            {
                new InventoryItem{BoardGameID=1,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=1,InStock=false,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=2,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=2,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=2,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=3,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=4,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=5,InStock=false,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=5,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=6,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=6,InStock=false,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=6,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=7,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=8,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=9,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=10,InStock=false,ReceivedDate=DateTime.Parse("2018-12-12"),ShippedDate=DateTime.Parse("2019-12-12"),Condition=InventoryItem.ConditionOptions.Used},
                new InventoryItem{BoardGameID=10,InStock=true,ReceivedDate=DateTime.Parse("2017-12-12"),ShippedDate=DateTime.Parse("2018-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=10,InStock=false,ReceivedDate=DateTime.Parse("2019-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
                new InventoryItem{BoardGameID=10,InStock=true,ReceivedDate=DateTime.Parse("2014-12-12"),ShippedDate=DateTime.Parse("2015-12-12"),Condition=InventoryItem.ConditionOptions.Used},
                new InventoryItem{BoardGameID=10,InStock=true,ReceivedDate=DateTime.Parse("2020-12-12"),ShippedDate=DateTime.Parse("2020-12-12"),Condition=InventoryItem.ConditionOptions.New},
            };

            context.InventoryItem.AddRange(inventoryItem);
            context.SaveChanges();

        }
    }
}
