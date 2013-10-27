using System.Collections.Generic;

namespace IocLite.SampleApp.Data
{
    public class VideoGameRepository : IVideoGameRepository
    {
        public IEnumerable<VideoGame> GetAllVideoGames()
        {
            return new List<VideoGame>
            {
                new VideoGame
                {
                    Name = "Grand Theft Auto V",
                    Description = "Los Santos: a sprawling sun-soaked metropolis full of self-help gurus, starlets and fading celebrities, once the envy of the Western world, now struggling to stay afloat in an era of economic uncertainty and cheap reality TV. Amidst the turmoil, three very different criminals plot their own chances of survival and success: Franklin, a former street gangster, now looking for real opportunities and serious money; Michael, a professional ex-con whose retirement is a lot less rosy than he hoped it would be; and Trevor, a violent maniac driven by the chance of a cheap high and the next big score. Running out of options, the crew risks everything in a series of daring and dangerous heists that could set them up for life.",
                    ConsolesAvailableFor = new List<VideoGameConsole>{ VideoGameConsole.Xbox360, VideoGameConsole.Playstation3},
                    PublisherName = "Rockstar"
                },
                new VideoGame
                {
                    Name = "Watch Dogs",
                    Description = "In today's hyper-connected world, You play as Aiden Pearce, a brilliant hacker but also a former thug, whose criminal past lead to a violent family tragedy. You'll be able to monitor and hack all who surround you while manipulating the city's systems to stop traffic lights, turn off the electrical grid and more.",
                    ConsolesAvailableFor = new List<VideoGameConsole>{ VideoGameConsole.Xbox360},
                   PublisherName = "Ubisoft"
                },
                new VideoGame
                {
                    Name = "Assassin's Creed",
                    Description = "It is 1715. Pirates rule the Caribbean and have established a lawless pirate republic. Among these outlaws is a fearsome young captain named Edward Kenway. His exploits earn the respect of pirate legends like Blackbeard, but draw him into an ancient war that may destroy everything the pirates have built. ",
                    ConsolesAvailableFor = new List<VideoGameConsole>{ VideoGameConsole.Xbox360, VideoGameConsole.Playstation3},
                    PublisherName = "Ubisoft"
                },
                new VideoGame
                {
                    Name = "Dead Rising",
                    Description = " Dead Rising follows the harrowing tale of Frank West, an overly zealous freelance photojournalist on a hunt for the scoop of a lifetime. In pursuit of a juicy lead, he makes his way to a small suburban town only to find that it has become overrun by zombies. He escapes to the local shopping mall, thinking it will be a bastion of safety, but it turns out to be anything but. Take the lead as Frank and engage in a struggle to survive the endless stream of enemies. With a full compliment of stores at your disposal in the mall, you'll need to be creative - utilizing anything you can find to fight off the flesh-hungry mob - and search for the truth behind the horrendous epidemic.",
                    ConsolesAvailableFor = new List<VideoGameConsole>{ VideoGameConsole.Xbox360, VideoGameConsole.Playstation3},
                    PublisherName = "Capcom"
                },
                new VideoGame
                {
                    Name = "LEGO Marvel Super Heroes Official Strategy Guide",
                    Description = "Be various Marvel Universe superheroes and save Earth, one brick at a time! ",
                    ConsolesAvailableFor = new List<VideoGameConsole>{ VideoGameConsole.Wii, VideoGameConsole.Xbox360, VideoGameConsole.Playstation3},
                    PublisherName = "Warner Brothers"
                }
            };
        }
    }

    public interface IVideoGameRepository
    {
        IEnumerable<VideoGame> GetAllVideoGames();
    }
}