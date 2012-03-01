using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
	class Reversi
	{
		public Board b = new Board();
		public AI ai;

		public static void Main()
		{
			Reversi game = new Reversi();

			game.play();
		}

		public void play()
		{
			ai = new AI(b);

			int safe = 0;
			Board.PIECE turn = Board.PIECE.BLACK;

			bool stuck_human = false, stuck_ai = false;
			while ((safe+=1) < 100)
			{
				b.print();
				
				if (turn == Board.PIECE.BLACK)
				{
					List<int> legal_cand = b.find_legal_candidates(turn);
					if (legal_cand.Count != 0)
					{
						int h = -1, w = -1;
						stuck_human = false;

						
						do
						{
							getInput(ref h, ref w);
						} while (!legalHumanInput(h, w, legal_cand));
						
						
						/*
						int index = ai.Static_H_Bot_AI(turn, legal_cand);
						h = index / Board.SIZE; w = index % Board.SIZE;
						*/

						b.place_piece(h, w, turn, Board.MARK.Mark);
					}
					else
					{
						stuck_human = true;
					}
					turn = Board.PIECE.WHITE;
				}
				else if (turn == Board.PIECE.WHITE)
				{
					List<int> legal_cand = b.find_legal_candidates(turn);
					if (legal_cand.Count != 0)
					{
						stuck_ai = false;

						// int index = ai.Random_AI(turn, legal_cand);
						//int index = ai.Simple_Bot_AI(turn, legal_cand);
						int index = ai.Static_H_Bot_AI(turn, legal_cand);
	
						int h = index / Board.SIZE, w = index % Board.SIZE;

						System.Console.WriteLine("-");
						Console.WriteLine(h + " " + w);
						Console.WriteLine();
						Console.WriteLine();

						b.place_piece(h, w, turn, Board.MARK.Mark);
					}
					else
					{
						stuck_ai = true;
					}
					turn = Board.PIECE.BLACK;
				}

				if (stuck_ai && stuck_human)
				{
					Console.WriteLine("Game over!");
					printResult();
					break;
				}
			}
		}

		bool legalHumanInput(int h, int w, List<int> legal_cand)
		{
			int input = h * Board.SIZE + w;
			return (legal_cand.IndexOf(input) != -1);
		}

		public void getInput(ref int h, ref int w)
		{
			System.Console.WriteLine("@");
			string[] tokens = Console.ReadLine().Split();
			h = int.Parse(tokens[0]);
			w = int.Parse(tokens[1]);
			System.Console.WriteLine();
			System.Console.WriteLine();
		}

		public void printResult()
		{
			int count_human = 0, count_ai = 0;
			for (int h = 0; h < Board.SIZE; h++)
			{
				for (int w = 0; w < Board.SIZE; w++)
				{
					if (b.board[h][w] == Board.PIECE.EMPTY) continue;
					else if (b.board[h][w] == Board.PIECE.BLACK) count_human++;
					else if (b.board[h][w] == Board.PIECE.WHITE) count_ai++;
				}
			}

			Console.WriteLine("Human " + count_human + "\n" + "AI " + count_ai + "\n");
		}
	}
}
