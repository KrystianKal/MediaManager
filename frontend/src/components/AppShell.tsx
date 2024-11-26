'use client'
import {useDisclosure} from "@mantine/hooks";
import {AppShell} from "@mantine/core";
import React from "react";
import {AppNavbar} from "@/src/components/AppNavbar";
import {AppHeader} from "@/src/components/AppShellHeader";

export function Shell({children} :{children:React.ReactNode}) {
  const [mobileOpened, {toggle: toggleMobile}] = useDisclosure();
  const [desktopOpened, {toggle: toggleDesktop}] = useDisclosure(true);
  return (
    <AppShell
      header={{ height: 60 }}
      navbar={{
        width: 300,
        breakpoint: 'sm',
        collapsed: { mobile: !mobileOpened, desktop: !desktopOpened },
      }}
      padding="md"
    >
      <AppHeader mobileOpened={mobileOpened} toggleMobile={toggleMobile} desktopOpened={desktopOpened} toggleDesktop={toggleDesktop} />
      <AppNavbar/>
      <AppShell.Main>
        {children} 
      </AppShell.Main>
    </AppShell>
  )
}

