using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Reversi
{
	class AI
	{
		public Board b;

		public List<List<Board.PIECE>> cur_board;
		public List<int> cur_empty_neighbor_list;

		public int SearchDepth = 4;

		public int best_option = -1;

		public double infinity = 100;

		public AI(Board board, List<List<Board.PIECE>> _cur_board, List<int> _cur_empty_neighbor_list)
		{
			b = board;
			cur_board = _cur_board;
			cur_empty_neighbor_list = _cur_empty_neighbor_list;
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
				int cur_gain = b.place_piece(cur_board, cur_empty_neighbor_list, h, w, caller, Board.MARK.Don_t_mark, weight);
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

			if (caller == Board.PIECE.BLACK)
			{
				max_value(copy(cur_board), new List<int>(cur_empty_neighbor_list), -infinity, infinity, SearchDepth);

				return legal_cand[best_option];
			}
			else if (caller == Board.PIECE.WHITE)
			{
				min_value(copy(cur_board), new List<int>(cur_empty_neighbor_list), -infinity, infinity, SearchDepth);

				return legal_cand[best_option];
			}

			return -1;
		}

		
		public double max_value(List<List<Board.PIECE>> state, List<int> empty_neighbor_list, double alpha, double beta, int d)
		{
			if (d == 0) return b.getStrengthRatio(state);

			List<int> legal_cand = b.find_legal_candidates(state, empty_neighbor_list, Board.PIECE.BLACK);

			if (legal_cand.Count == 0)
				return min_value(copy(state), new List<int>(empty_neighbor_list), alpha, beta, d - 1);
				
			double max_v = -infinity; int max_idx = -2;
			for (int i = 0; i < legal_cand.Count; i++)
			{
				int h = legal_cand[i] / Board.SIZE, w = legal_cand[i] % Board.SIZE;
				List<List<Board.PIECE>> branch_state = copy(state);
				List<int> branch_empty_neighbor_state = new List<int>(empty_neighbor_list);

				b.place_piece(branch_state, branch_empty_neighbor_state, h, w, Board.PIECE.BLACK, Board.MARK.Mark, Board.WEIGHT.Static);
				double cur_v = min_value(branch_state, branch_empty_neighbor_state, alpha, beta, d - 1);

				if (cur_v > max_v)
				{
					max_v = cur_v;
					max_idx = i;
				}
				
				if (max_v >= beta)
				{
					break;			// prune the rest
				}

				alpha = Math.Max(alpha, max_v);
			}

			best_option = max_idx;
			return max_v;
		}

		public double min_value(List<List<Board.PIECE>> state, List<int> empty_neighbor_list, double alpha, double beta,  int d)
		{
		//	b.print(state);
			if (d == 0) return b.getStrengthRatio(state);

			List<int> legal_cand = b.find_legal_candidates(state, empty_neighbor_list, Board.PIECE.WHITE);

			if (legal_cand.Count == 0)
				return max_value(copy(state), new List<int>(empty_neighbor_list), alpha, beta, d - 1);

			double min_v = infinity; int min_idx = -3;
			for (int i = 0; i < legal_cand.Count; i++)
			{
				int h = legal_cand[i] / Board.SIZE, w = legal_cand[i] % Board.SIZE;

				List<List<Board.PIECE>> branch_state = copy(state);
				List<int> branch_empty_neighbor_state = new List<int>(empty_neighbor_list);

				b.place_piece(branch_state, branch_empty_neighbor_state, h, w, Board.PIECE.WHITE, Board.MARK.Mark, Board.WEIGHT.Static);
				double cur_v = max_value(branch_state, branch_empty_neighbor_state, alpha, beta, d - 1);
					
				if (cur_v < min_v)
				{
					min_v = cur_v;
					min_idx = i;
				}
				
				if (min_v <= alpha)
				{
					break;
				}

				beta = Math.Min(beta, min_v);
			}

			best_option = min_idx;
			return min_v;
		}

		public static List<List<Board.PIECE>> copy(List<List<Board.PIECE>> from)
		{
			List<List<Board.PIECE>> result = new List<List<Board.PIECE>>();

			for (int h = 0; h < Board.SIZE; h++)
			{
				List<Board.PIECE> row = new List<Board.PIECE>();
				for (int w = 0; w < Board.SIZE; w++)
				{
					if (from[h][w] == Board.PIECE.BLACK) row.Add(Board.PIECE.BLACK);
					else if (from[h][w] == Board.PIECE.EMPTY) row.Add(Board.PIECE.EMPTY);
					else if (from[h][w] == Board.PIECE.WHITE) row.Add(Board.PIECE.WHITE);
				}
				result.Add(row);
			}
			return result;
		}
	}
}





/*
		public double max_value(List<List<Board.PIECE>> state, List<int> empty_neighbor_list, int d)
		{
			b.print(state);
			if (d == 0) return b.getStrengthRatio(state);

			List<int> legal_cand = b.find_legal_candidates(state, empty_neighbor_list, Board.PIECE.BLACK);

			if (legal_cand.Count == 0)
				return min_value(copy(state), new List<int>(empty_neighbor_list), d - 1);


			int index = Static_H_Bot_AI(Board.PIECE.BLACK, legal_cand);
			int h = index / Board.SIZE, w = index % Board.SIZE;

			List<List<Board.PIECE>> branch_state = copy(state);
			List<int> branch_empty_neighbor_state = new List<int>(empty_neighbor_list);

			b.place_piece(branch_state, branch_empty_neighbor_state, h, w, Board.PIECE.BLACK, Board.MARK.Mark, Board.WEIGHT.Static);
			return min_value(branch_state, branch_empty_neighbor_state, d - 1);
		}
		*/