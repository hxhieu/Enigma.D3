using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Enigma.Wpf;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using Enigma.D3.Enums;
using System.Windows.Media.Media3D;
using Enigma.D3.MapHack.Markers;
using Enigma.D3.MemoryModel;
using Enigma.D3.MemoryModel.Core;

namespace Enigma.D3.MapHack
{
	internal class Minimap : NotifyingObject
	{
		private Canvas _window;
		private Canvas _root;
		private MinimapControl _minimapControl;
		private ObservableCollection<IMapMarker> _minimapItems;

		public Minimap(Canvas overlay)
		{
			_minimapItems = new ObservableCollection<IMapMarker>();

			_root = new Canvas() { Height = 1200.5d };
			_window = overlay;
			_window.Children.Add(_root);
			_window.SizeChanged += (s, e) => UpdateSizeAndPosition();

			_root.Children.Add(_minimapControl = new MinimapControl { DataContext = this });

			UpdateSizeAndPosition();
		}

		public ObservableCollection<IMapMarker> MinimapMarkers { get { return _minimapItems; } }

		private void UpdateSizeAndPosition()
		{
			var uiScale = _window.ActualHeight / 1200d;
			_root.Width = _window.ActualWidth / uiScale;
			_root.RenderTransform = new ScaleTransform(uiScale, uiScale, 0, 0);
		}

		private Dictionary<int, IMapMarker> _minimapItemsDic = new Dictionary<int, IMapMarker>();
		private int _lastFrame;
		private HashSet<int> _ignoredSnoIds = new HashSet<int>();
		private ACD _playerAcd;

		private LocalData _localData;
		private ObjectManager _objectManager;

		private bool _isLocalActorReady;
		private ContainerObserver<ACD> _acdsObserver;

		public void Update(Engine engine)
		{
			try
			{
				if (!IsLocalActorValid(engine))
					return;

				if (!IsObjectManagerOnNewFrame(engine))
					return;

				var itemsToAdd = new List<IMapMarker>();

				if (_acdsObserver == null)
				{
					var acds = _objectManager.ACDManager.ActorCommonData;
					_acdsObserver = new ContainerObserver<ACD> { Container = acds };
				}

				_acdsObserver.Update();
				var playerAcdId = _objectManager.PlayerDataManager[_objectManager.Player.LocalPlayerIndex].ACDID;
				_playerAcd = _acdsObserver.CurrentItems.First(x => x.ID == playerAcdId);
				
				foreach (var index in _acdsObserver.AddedItems)
				{
					var acd = _acdsObserver.CurrentItems.First(x => (short)x.ID == index);
					if (_minimapItemsDic.ContainsKey(acd.Address))
						continue;

					int acdId = acd.ID;
					if (acdId == -1)
						continue;
					
					var actorSnoId = acd.ActorSNO;
					if (_ignoredSnoIds.Contains(actorSnoId))
						continue;

					if (!_minimapItemsDic.ContainsKey(acd.Address))
					{
						bool ignore;
						var minimapItem = MapMarkerFactory.Create(acd, out ignore);
						if (ignore)
						{
							_ignoredSnoIds.Add(actorSnoId);
						}
						else if (minimapItem != null)
						{
							_minimapItemsDic.Add(acd.Address, minimapItem);
							itemsToAdd.Add(minimapItem);
						}
					}
				}

				UpdateUI(itemsToAdd);
			}
			catch (Exception exception)
			{
				_acdsObserver = null;
				OnUpdateException(exception);
			}
		}

		private bool IsLocalActorValid(Engine engine)
		{
			_localData = _localData ?? engine.Context.DataSegment.LocalData;

			var isNotInGame = _localData.IsStartUpGame;
			//byte isActorCreated = (byte)_localData.x00_IsActorCreated;
			if (_localData.ActID == unchecked((int)0xCDCDCDCD)) // structure is being updated, everything is cleared with 0xCD ('-')
			{
				if (!_isLocalActorReady)
					return false;
			}
			else
			{
				if (!isNotInGame)
				{
					if (!_isLocalActorReady)
					{
						_isLocalActorReady = true;
						OnLocalActorCreated();
					}
				}
				else
				{
					if (_isLocalActorReady)
					{
						_isLocalActorReady = false;
						OnLocalActorDisposed();
					}
					return false;
				}
			}
			return true;
		}

		private bool IsObjectManagerOnNewFrame(Engine engine)
		{
			_objectManager = _objectManager ?? engine.Context.DataSegment.ObjectManager;

			// Don't do anything unless game updated frame.
			int currentFrame = _objectManager.RenderTick;
			if (currentFrame == _lastFrame)
				return false;
			if (currentFrame < _lastFrame)
			{
				// Lesser frame than before = left game probably.
				_playerAcd = null;
				_lastFrame = currentFrame;
				return false;
			}
			_lastFrame = currentFrame;
			return true;
		}

		private void OnUpdateException(Exception exception)
		{
			Trace.WriteLine(exception.Message);
		}

		private void UpdateUI(List<IMapMarker> itemsToAdd)
		{
			var itemsToRemove = new List<IMapMarker>();
			foreach (var mapItem in _minimapItems.Concat(itemsToAdd))
			{
				if (!mapItem.Update(_playerAcd.WorldSNO, new Point3D
				{
					X = _playerAcd.Position.X,
					Y = _playerAcd.Position.Y,
					Z = _playerAcd.Position.Z
				}))
				{
					itemsToRemove.Add(mapItem);
					_minimapItemsDic.Remove(mapItem.Id);
				}
			}

			if (itemsToAdd.Count > 0 || itemsToRemove.Count > 0)
			{
				if (itemsToAdd.Count > 0)
					Trace.WriteLine("Adding " + itemsToAdd.Count + " items...");
				if (itemsToRemove.Count > 0)
					Trace.WriteLine("Removing " + itemsToRemove.Count + " items...");

				Execute.OnUIThread(() =>
				{
					itemsToAdd.ForEach(a => _minimapItems.Add(a));
					itemsToRemove.ForEach(a => _minimapItems.Remove(a));
				});
			}
		}

		private void OnLocalActorCreated()
		{
			Trace.WriteLine("Local Actor Ready");
		}

		private void OnLocalActorDisposed()
		{
			Trace.WriteLine("Local Actor Not Ready");
			if (_minimapItemsDic.Count > 0)
				_minimapItemsDic.Clear();
			if (_minimapItems.Count > 0)
				Execute.OnUIThread(() => _minimapItems.Clear());
		}
	}
}
