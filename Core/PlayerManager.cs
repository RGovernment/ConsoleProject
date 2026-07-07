using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGameFramework.Models;

namespace ConsoleGameFramework.Core;

public class PlayerManager
{
    public static PlayerManager Instance { get; } = new PlayerManager();

    public Player playerStatus { get; set; }

    private PlayerManager()
    {

    }

    public PlayerManager(Player player)
    {
        playerStatus = player;
    }

}
