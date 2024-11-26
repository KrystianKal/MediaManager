import {MediaView} from "@/src/features/media/components/Media/MediaView";

export default async function Page({params}: {params: Promise<{id:string}>}) {
  const id = (await params).id
  
  return <>
    <MediaView mediaId={id}/>
  </>
}

