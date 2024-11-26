'use client'
import {Image, Loader} from "@mantine/core";
import {API_URL} from "@/src/lib/constants";
import {getMediaById} from "@/src/features/media/actions";
import {MediaVideo} from "@/src/features/media/components/Media/MediaVideo";
import {useEffect, useState} from "react";
import {Media} from "@/src/features/media/Media";
import {notFound} from "next/navigation";

type MediaProps = {mediaId: string }
export function MediaView({mediaId}: MediaProps) {
  const [loading, setLoading] = useState(true);
  const [currentMedia,setCurrentMedia] = useState<Media | null>(null);
  
  useEffect(()=>{
    (async () => {
      const data = await getMediaById(mediaId);
      if(!data) notFound()
      setCurrentMedia(data)
      setLoading(false);
    })()
  },[])
  
  if (loading ) return <Loader/>
  
  const mediaComponent = currentMedia?.type === 'video'
    ? <MediaVideo url={`${API_URL}${currentMedia?.fullName}`}/>
    : <Image width={'100%'} height={'auto'} src={`${API_URL}${currentMedia?.fullName}`}/>
  //todo image for image, video for videos
  return mediaComponent
  
}
