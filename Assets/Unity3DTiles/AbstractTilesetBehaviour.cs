﻿/*
 * Copyright 2018, by the California Institute of Technology. ALL RIGHTS 
 * RESERVED. United States Government Sponsorship acknowledged. Any 
 * commercial use must be negotiated with the Office of Technology 
 * Transfer at the California Institute of Technology.
 * 
 * This software may be subject to U.S.export control laws.By accepting 
 * this software, the user agrees to comply with all applicable 
 * U.S.export laws and regulations. User has the responsibility to 
 * obtain export licenses, or other export authority as may be required 
 * before exporting such information to foreign countries or providing 
 * access to foreign persons.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Unity3DTiles
{
    public class AbstractTilesetBehaviour : MonoBehaviour
    {
        public Unity3DTilesetSceneOptions SceneOptions = new Unity3DTilesetSceneOptions();
        public Unity3DTilesetStatistics Stats;
        public LRUCache<Unity3DTile> LRUCache = new LRUCache<Unity3DTile>();
        protected Queue<Unity3DTile> postDownloadQueue = new Queue<Unity3DTile>();
        protected RequestManager requestManager;

        public int MaxConcurrentRequests = 6;

        public void LateUpdate()
        {
            LRUCache.MaxSize = SceneOptions.LRUCacheMaxSize;
            LRUCache.MarkAllUnused();
            this._lateUpdate();
            this.requestManager?.Process();
            // Move any tiles with downloaded content to the ready state
            int processed = 0;
            while (processed < this.SceneOptions.MaximumTilesToProcessPerFrame && this.postDownloadQueue.Count != 0)
            {
                var tile = this.postDownloadQueue.Dequeue();
                // We allow requests to terminate early if the (would be) tile goes out of view, so check if a tile is actually processed
                if (tile.Process())
                {
                    processed++;
                }
            }
            LRUCache.UnloadUnusedContent(SceneOptions.LRUCacheTargetSize, SceneOptions.LRUMaxFrameUnloadRatio, n => -n.Depth, t => t.UnloadContent());
            this.updateStats();
        }

        protected virtual void updateStats()
        {
            //override in subclass
        }

        protected virtual void _lateUpdate()
        {
            //override in subclass
        }

        public void Start()
        {
            _start();
        }

        protected virtual void _start()
        {
            //override in subclass
        }
    }
}