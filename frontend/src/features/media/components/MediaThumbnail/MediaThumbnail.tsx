'use client'
import {useHover} from "@mantine/hooks";
import {useEffect, useState} from "react";
import {Media} from "@/src/features/media/Media";
import {Box, Checkbox, Image, Text} from "@mantine/core";
import styles from "./MediaThumbnail.module.css"
import {formatVideoLength} from "@/src/features/media/utils";
import {API_URL} from "@/src/lib/constants";
import {useMediaGrid} from "@/src/features/media/MediaGridContext";
import {useRouter} from "next/navigation";

function advanceThumbnailIndex (prev:number, thumbnailCount:number) {
  const next_index: number = prev +1;
  const wasLast: boolean = next_index === thumbnailCount;
  return wasLast ? 0 : next_index;
}

function useHoverThumbnail (thumbnails: string[], hovered:boolean) {
  const [currentIndex, setCurrentIndex] = useState(0);
  useEffect(() => {
    if (!hovered) {
      setCurrentIndex(0);
      return;
    }
    if (thumbnails.length <= 1) return

    const interval = setInterval(() => {
      setCurrentIndex( (prev) =>
        advanceThumbnailIndex(prev, thumbnails.length)
      );
    },750);
    
    return () => clearInterval(interval);
  }, [hovered])
  return currentIndex;
}
type MediaSelectionCheckboxProps = {isSelected:boolean,isFocused:boolean,isSelectionMode: boolean, onChange:() => void}
function MediaSelectionCheckbox({isSelected,isFocused,isSelectionMode, onChange}: MediaSelectionCheckboxProps ) {
  return (
    <Checkbox checked={isSelected}
              onChange={e => {
                e.stopPropagation();
                onChange()
              } }
              onClick={e => e.stopPropagation()}
              style={{
                opacity: isSelected ? 0.85 : 0.4,
                visibility: (isFocused || isSelectionMode ) ? "visible" : "hidden",
                transition: 'opacity 0.2s ease'
              }}
              className={`${styles.overlay} ${styles.overlayTopLeft} ${styles.checkbox}`}  />
  )
}
export function MediaThumbnail({media}: { media: Media}){
  const {hovered, ref} = useHover<HTMLImageElement>();
  const {selectedMedia, toggleSelection, isSelectionMode} = useMediaGrid();
  const isSelected = selectedMedia.has(media.id);
  const currentThumbnailIndex = useHoverThumbnail(media.thumbnails,hovered);
  const router = useRouter();
  return (
    <Box>
      <Box
        ref={ref}
        className={styles.thumbnailWrapper} 
        //todo use cache to avoid fetching 
        onClick={() => router.push(`/media/${media.id}`)}
      >
        
      <Image 
        src={`${API_URL}${media.thumbnails[currentThumbnailIndex]}`}
        className={styles.thumbnail}
      />
      {
        media.durationSeconds && (
        <Text className={`${styles.overlay} ${styles.overlayBottomRight}`}>{formatVideoLength(media.durationSeconds)}</Text>
        )
      }
      <MediaSelectionCheckbox 
        isSelected={isSelected} 
        isFocused={hovered} 
        isSelectionMode={isSelectionMode} 
        onChange={() => toggleSelection(media.id)} />
      </Box>
    </Box>
  )
}
