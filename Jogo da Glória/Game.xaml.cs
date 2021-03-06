﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// for Task class

// for Thread class

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Jogo_da_Glória
{
    /// <summary>
    ///     The actual game page.
    /// </summary>
    public sealed partial class Game : Page
    {
        public static AutoResetEvent arEvent = new AutoResetEvent(false);
        private readonly List<Player> allPlayers = new List<Player>();

        private readonly Dictionary<string, int> allPositions = new Dictionary<string, int>();

        public int maxPlayers, currentPlayer = 1;

        private int seed;

        public Game()
        {
            InitializeComponent();
            Loaded += (s, e) => { AwaitForButtonClicks(); };
        }


        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.
        ///     This parameter is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.Assert(e.Parameter != null, "e.Parameter != null");
            maxPlayers = (int) e.Parameter;
            Debug.WriteLine("There will be " + maxPlayers + " players in this game");

            #region gameText

            allPositions.Add("Ínicio!\n Beba e jogue o dado!", 0);
            allPositions.Add("Todos bebem!", 0);
            allPositions.Add("Aponte para a pessoa que irá beber.", 0);
            allPositions.Add("Você está com sorte! Você bebe.", 0);
            allPositions.Add("O jogador que está á sua esquerda bebe!", 0);
            allPositions.Add("Mande dois jogadores beberem!", 0);
            allPositions.Add("Todos bebem menos você", 0);
            allPositions.Add("Beba a quantia de dose correspondente ao número no dado", 0);
            allPositions.Add("Náo faça nada.", 0);
            allPositions.Add("Beba e volte 7 casas", -7);
            allPositions.Add("O dono do jogo bebe 3", 0);
            allPositions.Add("Todas as mulheres bebem.", 0);
            allPositions.Add("Todos voltam uma casa e bebem", -1);
            allPositions.Add("Todos os jogadores sem C,N ou H no nome bebem", 0);
            allPositions.Add("Beba 3 OU volta 8 casas.", 0);
            allPositions.Add("Jogue dado novamente, beba o número que ele mostrar", 0);
            allPositions.Add("Mande 3 jogadores beberem", 0);
            allPositions.Add("Os jogadores do seu lado bebem", 0);
            allPositions.Add("Todos os homem bebem", 0);
            allPositions.Add("Todos os jogadores usando preto bebem.", 0);
            allPositions.Add("A mulher que você apontar bebe 3", 0);
            allPositions.Add("Volte 7 casas", -7);
            allPositions.Add("Todos jogaram o dado, quem tirou 6 VOLTOU ao ÍNICIO", 0);
            allPositions.Add("Pode ir a casa de banho", 0);
            allPositions.Add("Jogou uma moeda\n", 0);
            allPositions.Add("Beba! Já agora, voltou para trás ", 0);
            allPositions.Add("Conte uma piada ou beba 4", 0);
            allPositions.Add("Beba e fica uma rodada sem jogar", 0);
            allPositions.Add("Beba com o DONO do jogo!", 0);
            allPositions.Add("Beba e volte 7 casas!", -7);
            allPositions.Add("Faça um brinde com todos os jogadores e bebam", 0);
            allPositions.Add("FIM!", 0);

            #endregion

            for (var i = 0; i < maxPlayers; i++)
            {
                allPlayers.Add(new Player(i + 1, 0)); //Initializes all players with their Id and Current Position
                Debug.WriteLine("Player added:" + allPlayers.ElementAt(i));
            }

            txt_current_player.Text = "Jogador " + allPlayers.ElementAt(0).Id;
            txt_instructions.Text = allPositions.ElementAt(0).Key;
            allPlayers.ElementAt(0).FirstTime = false; //Would never get flagged to false else
        }

        private void textBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var r = new Random();
            if (allPlayers.Count == 0) // Game ended
            {
                txt_current_player.Text = "";
                txt_dice.Text = "";
                txt_instructions.Text = "O jogo acabou! Parabéns seus bebados!!";
                button.Visibility = Visibility.Collapsed;
                bt_go.Visibility = Visibility.Visible;
                return;
            }

            if (currentPlayer == allPlayers.Count)
                currentPlayer = 1;
            else
                currentPlayer = currentPlayer + 1;

            var player = allPlayers.ElementAt(currentPlayer - 1);
            Debug.WriteLine(player);

            if (!player.CanPlay)
            {
                player.CanPlay = true;
                return;
            }


            if (!player.FirstTime)
            {
                //If not start
                //Get seed of dice

                do
                {
                    seed = r.Next(7);
                } while (seed == 0);
                txt_dice.Text = "O Dado rolou um " + seed;
                //Cant be 0 and must be less than 7
                player.CurrentPos = player.CurrentPos + seed;
                Debug.WriteLine(player);
                txt_current_player.Text = "Jogador " + player.Id + " Casa " + player.CurrentPos;
                txt_instructions.Text = allPositions.ElementAt(player.CurrentPos).Key;
                switch (player.CurrentPos)
                {
                    default:
                        break;
                    case 14:
                        bt_no.Visibility = Visibility.Visible;
                        bt_yes.Visibility = Visibility.Visible;
                        button.IsEnabled = false;
                        break;
                    case 15:
                        do
                        {
                            seed = r.Next(7);
                        } while (seed == 0);
                        txt_instructions.Text = txt_instructions.Text + "\n O dado rodou um " + seed;
                        break;
                    case 22:
                        foreach (var p in allPlayers)
                        {
                            do
                            {
                                seed = r.Next(7);
                            } while (seed == 0);
                            if (seed == 6)
                                p.CurrentPos = 0;
                        }
                        break;
                    case 24:
                        seed = r.Next(2);
                        if (seed == 0)
                            txt_instructions.Text = txt_instructions.Text + "\nCARA: Só voce bebe";
                        else
                            txt_instructions.Text = txt_instructions.Text + "\nCOROA: Todos bebem";
                        break;
                    case 25:
                        do
                        {
                            seed = r.Next(7);
                        } while (seed == 0);
                        player.CurrentPos = player.CurrentPos - seed;
                        txt_instructions.Text = txt_instructions.Text + seed + " casas!";
                        break;
                    case 26:
                        bt_joke.Visibility = Visibility.Visible;
                        bt_drink.Visibility = Visibility.Visible;
                        button.IsEnabled = false;
                        break;
                    case 27:
                        player.CanPlay = false;
                        return;
                }
                Debug.WriteLine(player);
                player.CurrentPos = player.CurrentPos + allPositions.ElementAt(player.CurrentPos).Value;
                txt_current_player.Text = "Jogador " + player.Id + " Casa " + player.CurrentPos;
            }
            else
            {
                player.FirstTime = false;
            }

            if (player.CurrentPos.Equals(30))
            {
                Debug.WriteLine("Player" + player + " ended the game");
                var msbox = new MessageDialog("O jogador " + player.Id + " terminou o jogo!");
                await msbox.ShowAsync();
                if (currentPlayer == allPlayers.Last().Id)
                    currentPlayer = 1;
                allPlayers.RemoveAll(x => x.Id.Equals(player.Id));
            }
            else
                Debug.WriteLine(player);
        }

        private async void AwaitForButtonClicks()
        {
            await bt_no;
            await bt_yes;
        }

        private void bt_no_Click(object sender, RoutedEventArgs e)
        {
            var player = allPlayers.ElementAt(currentPlayer - 1);
            player.CurrentPos = player.CurrentPos - 8;
            Debug.WriteLine("YOU CHOSE TO GO BACK");
            bt_yes.Visibility = Visibility.Collapsed;
            bt_no.Visibility = Visibility.Collapsed;
            button.IsEnabled = true;
        }

        private void bt_yes_Click(object sender, RoutedEventArgs e)
        {
            var player = allPlayers.ElementAt(currentPlayer - 1);
            player.DrinksCounter = player.DrinksCounter + 8;
            Debug.WriteLine("YOU CHOSE TO DRINK");
            bt_yes.Visibility = Visibility.Collapsed;
            bt_no.Visibility = Visibility.Collapsed;
            button.IsEnabled = true;
        }

        private void bt_go_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
        }

        private void bt_drink_Click(object sender, RoutedEventArgs e)
        {
            var player = allPlayers.ElementAt(currentPlayer - 1);
            player.DrinksCounter = player.DrinksCounter + 4;
            Debug.WriteLine("YOU CHOSE TO DRINK");
            bt_drink.Visibility = Visibility.Collapsed;
            bt_joke.Visibility = Visibility.Collapsed;
            button.IsEnabled = true;
        }

        private void bt_joke_Click(object sender, RoutedEventArgs e)
        {
            bt_drink.Visibility = Visibility.Collapsed;
            bt_joke.Visibility = Visibility.Collapsed;
            button.IsEnabled = true;
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }
    }
}