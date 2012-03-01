using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
	class AI
	{
		public Board b;

		public AI(Board board)
		{
			b = board;
		}

		public int Random_AI(Board.PIECE caller, List<int> legal_cand)
		{
			Random r = new Random();
			int random_idx = r.Next(legal_cand.Count);
			return legal_cand[random_idx];
		}

		// Greedy; Choose the Move with Maximum Immediate Gain.
		public int Generic_Greedy_AI(Board.PIECE caller, List<int> legal_cand, Board.WEIGHT weight)
		{
			int max_gain = -10000; int max_gain_idx = -1;

			for (int i = 0; i < legal_cand.Count; i++)
			{
				int h = legal_cand[i] / Board.SIZE, w = legal_cand[i] % Board.SIZE;
				int cur_gain = b.place_piece(h, w, caller, Board.MARK.Don_t_mark, weight);
				if (cur_gain > max_gain)
				{
					max_gain = cur_gain;
					max_gain_idx = i;
				}
			}

			return legal_cand[max_gain_idx];
		}

		public int Simple_Bot_AI(Board.PIECE caller, List<int> legal_cand)
		{
			return Generic_Greedy_AI(caller, legal_cand, Board.WEIGHT.Default);
		}

		public int Static_H_Bot_AI(Board.PIECE caller, List<int> legal_cand)
		{
			return Generic_Greedy_AI(caller, legal_cand, Board.WEIGHT.Static);
		}

		public int Alpha_Beta_AI(Board.PIECE caller, List<int> legal_cand)
		{



			return -1;
		}
	}
}
