'use client'
import {getAllMedia} from "@/src/features/media/actions";
import {Loader, SimpleGrid} from "@mantine/core"
import {MediaThumbnail} from "@/src/features/media/components/MediaThumbnail/MediaThumbnail";
import {Media} from "@/src/features/media/Media";
import {useEffect, useState} from "react";
import {MediaContextProvider} from "@/src/features/media/MediaGridContext";
import styles from "./MediaGrid.module.css"
import {useViewportSize} from "@mantine/hooks";
import {MediaGridControls} from "@/src/features/media/components/MediaGrid/MediaGridControls/MediaGridControls";
import {useSearchParams} from "next/navigation";
import {GRID_TYPES} from "@/src/features/media/components/MediaGrid/MediaGrid";
import MediaThumbnailMenu from "@/src/features/media/components/MediaThumbnailMenu";


export function MediaGridClient({initialMedia}:{initialMedia:Media[]}) {
  const [media] = useState<Media[]>(initialMedia);
  const searchParams = useSearchParams()
  let flow = searchParams.get('flow')
  
  const gridType =  flow
    ? GRID_TYPES[Number(flow) as keyof typeof GRID_TYPES] || "Masonry"
    : "Masonry";

  const grid = gridType === "Masonry"
    ? <MasonryGrid media={media}/>
    : <EvenGrid media={media}/>

  return (
    <MediaContextProvider>
      <MediaGridControls/>
      {grid}
    </MediaContextProvider>
  )
}
//todo add cache, remove the need to fetch since all media are already im memory
function EvenGrid ({media}: {media:Media[]}) {
  return(
    <SimpleGrid className={styles.evenGrid}>
      {media.map( (m,idx) =>
          <MediaThumbnail key={idx}  media={m} ></MediaThumbnail>
      )}
    </SimpleGrid>
  )
}
function MasonryGrid({media}: {media:Media[]}) {
  const {width} = useViewportSize();
  const [cols,setCols] = useState(5);
  useEffect(() => {
    //todo base on image width
    if(width < 768) setCols(2);
    else if(width < 992) setCols(3);
    else if(width < 1200) setCols(4);
    else setCols(5);
  },[width])
  return(
    <div style={{ columnCount: cols }} className={styles.denseGrid}>
      {media.map( (m,idx) =>
          <MediaThumbnail key={idx} media={m}></MediaThumbnail>
      )}
    </div>
  )
}
