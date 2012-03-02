using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
	class AI
	{
		public Board b;

		public int SearchDepth = 5;

		public int best_option = -1;

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

		
		public int Mini_Max_AI(Board.PIECE caller, List<int> legal_cand)
		{
			// black will maximize
			// white will minimize

			List<List<Board.PIECE>> state = copyState(b.board);
			List<int> empty_neighbors_list = new List<int>(b.empty_neighbors_list);

			if (caller == Board.PIECE.BLACK)
			{
				max_value(SearchDepth);

				b.board = copyState(state);
				b.empty_neighbors_list = new List<int>(empty_neighbors_list);

				return legal_cand[best_option];
			}
			else if (caller == Board.PIECE.WHITE)
			{
				min_value(SearchDepth);

				b.board = copyState(state);
				b.empty_neighbors_list = new List<int>(empty_neighbors_list);

				return legal_cand[best_option];
			}

			return -1;
		}

		public double max_value(int d)
		{
			if (d == 0) return b.getStrengthRatio();

			List<List<Board.PIECE>> state = copyState(b.board);
			List<int> empty_neighbors_list = new List<int>(b.empty_neighbors_list);
			List<int> legal_cand = b.find_legal_candidates(Board.PIECE.BLACK);

			if (legal_cand.Count == 0)	return min_value(d - 1);
				
			double max_v = -1000; int max_idx = -1;
			for (int i = 0; i < legal_cand.Count; i++)
			{
				b.board = copyState(state);
				b.empty_neighbors_list = new List<int>(empty_neighbors_list);

				int h = legal_cand[i] / Board.SIZE, w = legal_cand[i] / Board.SIZE;
				b.place_piece(h, w, Board.PIECE.BLACK, Board.MARK.Mark, Board.WEIGHT.Static);
				double cur_v = min_value(d - 1);

				if (cur_v > max_v)
				{
					max_v = cur_v;
					max_idx = i;
				}
			}

			best_option = max_idx;
			return max_v;
		}

		public double min_value(int d)
		{
			if (d == 0) return b.getStrengthRatio();

			List<List<Board.PIECE>> state = copyState(b.board);
			List<int> empty_neighbors_list = new List<int>(b.empty_neighbors_list);
			List<int> legal_cand = b.find_legal_candidates(Board.PIECE.WHITE);

			if (legal_cand.Count == 0) return b.getStrengthRatio();

			double min_v = 1000; int min_idx = -1;
			for (int i = 0; i < legal_cand.Count; i++)
			{
				b.board = copyState(state);
				b.empty_neighbors_list = new List<int>(empty_neighbors_list);

				int h = legal_cand[i] / Board.SIZE, w = legal_cand[i] / Board.SIZE;
				b.place_piece(h, w, Board.PIECE.WHITE, Board.MARK.Mark, Board.WEIGHT.Static);
				double cur_v = max_value(d - 1);

				if (cur_v < min_v)
				{
					min_v = cur_v;
					min_idx = i;
				}
			}

			best_option = min_idx;
			return min_v;
		}

		public List<List<Board.PIECE>> copyState(List<List<Board.PIECE>> from)
		{
			List<List<Board.PIECE>> result = new List<List<Board.PIECE>>();

			for (int h = 0; h < Board.SIZE; h++)
			{
				List<Board.PIECE> row = new List<Board.PIECE>();
				for (int w = 0; w < Board.SIZE; w++)
				{
					if(from[h][w] == Board.PIECE.BLACK) row.Add(Board.PIECE.BLACK);
					else if(from[h][w] == Board.PIECE.EMPTY) row.Add(Board.PIECE.EMPTY);
					else if(from[h][w] == Board.PIECE.WHITE) row.Add(Board.PIECE.WHITE);
				}
				result.Add(row);
			}
			return result;
		}
	}
}
