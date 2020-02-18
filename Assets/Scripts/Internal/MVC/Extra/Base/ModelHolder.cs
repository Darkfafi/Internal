using System;
using System.Collections.Generic;

namespace MVC
{
	public class ModelHolder<BM> where BM : BaseModel
	{
		public event Action<BM> TrackedEvent;
		public event Action<BM> UntrackedEvent;

		private List<BM> _models = new List<BM>();

		// -- Entity Query Methods -- \\

		// - Single Entity - \\

		public BM GetRandom()
		{
			return GetRandom<BM>();
		}

		public BM GetRandom(Func<BM, bool> filterCondition)
		{
			return GetRandom<BM>(filterCondition);
		}

		public BM GetRandom<T>() where T : BM
		{
			return GetRandom<BM>(null);
		}

		public T GetRandom<T>(Func<T, bool> filterCondition) where T : BM
		{
			Random r = new Random();
			T[] e = GetAll(filterCondition);
			if (e.Length > 0)
				return e[r.Next(0, e.Length)];

			return null;
		}

		public BM GetFirst(Comparison<BM> sort = null)
		{
			return GetFirst<BM>(sort);
		}

		public BM GetFirst(Func<BM, bool> filterCondition, Comparison<BM> sort = null)
		{
			return GetFirst<BM>(filterCondition, sort);
		}

		public T GetFirst<T>(Comparison<T> sort = null) where T : BM
		{
			return GetFirst(null, sort);
		}

		public T GetFirst<T>(Func<T, bool> filterCondition, Comparison<T> sort = null) where T : BM
		{
			T[] e = GetAll(filterCondition, sort);
			if (e.Length > 0)
				return e[0];

			return null;
		}

		// - Multiple Entities - \\

		public void ForEach(Action<BM> method, Comparison<BM> sort = null)
		{
			ForEach<BM>(method, sort);
		}

		public void ForEach(Action<BM> method, Func<BM, bool> filterCondition, Comparison<BM> sort = null)
		{
			ForEach<BM>(method, filterCondition, sort);
		}

		public void ForEach<T>(Action<T> method, Comparison<T> sort = null) where T : BM
		{
			ForEach(method, null, sort);
		}

		public void ForEach<T>(Action<T> method, Func<T, bool> filterCondition, Comparison<T> sort = null) where T : BM
		{
			T[] all = GetAll(filterCondition, sort);
			for (int i = 0, c = all.Length; i < c; i++)
			{
				method(all[i]);
			}
		}

		public void ForEachReverse(Action<BM> method, Comparison<BM> sort = null)
		{
			ForEachReverse<BM>(method, sort);
		}

		public void ForEachReverse(Action<BM> method, Func<BM, bool> filterCondition, Comparison<BM> sort = null)
		{
			ForEachReverse<BM>(method, filterCondition, sort);
		}

		public void ForEachReverse<T>(Action<T> method, Comparison<T> sort = null) where T : BM
		{
			ForEachReverse(method, null, sort);
		}

		public void ForEachReverse<T>(Action<T> method, Func<T, bool> filterCondition, Comparison<T> sort = null) where T : BM
		{
			T[] all = GetAll(filterCondition, sort);
			for (int i = all.Length - 1; i >= 0; i--)
			{
				method(all[i]);
			}
		}

		public BM[] GetAll(Comparison<BM> sort = null)
		{
			if (sort == null)
				return _models.ToArray();
			else
				return GetAll<BM>(sort);
		}

		public BM[] GetAll(Func<BM, bool> filterCondition, Comparison<BM> sort = null)
		{
			return GetAll<BM>(filterCondition, sort);
		}

		public T[] GetAll<T>(Comparison<T> sort = null) where T : BM
		{
			return GetAll(null, sort);
		}

		public T[] GetAll<T>(Func<T, bool> filterCondition, Comparison<T> sort = null) where T : BM
		{
			List<T> result = new List<T>();
			for (int i = 0, count = _models.Count; i < count; i++)
			{
				T e = _models[i] as T;
				if (e != null && (filterCondition == null || filterCondition(e)))
				{
					result.Add(e);
				}
			}

			if (sort != null)
				result.Sort(sort);

			return result.ToArray();
		}

		public bool Has(BM model)
		{
			return _models.Contains(model);
		}

		public virtual void Clean()
		{
			for (int i = _models.Count - 1; i >= 0; i--)
			{
				Untrack(_models[i]);
			}

			_models.Clear();
			_models = null;
		}

		// Internal tracking

		protected bool Track(BM model)
		{
			if (_models.Contains(model))
				return false;

			_models.Add(model);

			if (TrackedEvent != null)
			{
				TrackedEvent(model);
			}

			return true;
		}

		protected bool Untrack(BM model)
		{
			if (!_models.Contains(model))
				return false;

			_models.Remove(model);

			if (UntrackedEvent != null)
			{
				UntrackedEvent(model);
			}

			return true;
		}
	}
}