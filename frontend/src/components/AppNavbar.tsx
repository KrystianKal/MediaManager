'use client'
import {AppShell, NavLink} from "@mantine/core";
import React, {ReactElement} from "react";
import {IconArchive, IconFolders, IconHome2} from "@tabler/icons-react";
import Link from "next/link";
import {usePathname} from "next/navigation";

//skeleton
// {Array(15)
//   .fill(0)
//   .map((_, index) => (
//     <Skeleton key={index} h={28} mt="sm" animate={false} />
//   ))}
interface navLink {href: string; label:string; icon: ReactElement<any,any>}
export function AppNavbar () {
  const pathName = usePathname()
  
  const navLinks: navLink[] = 
    [
      {
        href: '/',
        label: 'Home',
        icon: <IconHome2 stroke={1.5}/>
      },
      {
        href: '/directories',
        label: 'Directories',
        icon: <IconFolders stroke={1.5}/>
      },
      {
        href: '/collections',
        label: 'Collections',
        icon: <IconArchive  stroke={1.5}/>
      }
    
    ]

  //todo going to home should not refetch data
  return(
  <AppShell.Navbar p="md">
    {navLinks.map( navLink => (
      <Link href={navLink.href} key={navLink.label} passHref legacyBehavior={true}>
        <NavLink 
          component={'a'}  
          label={navLink.label} 
          leftSection={navLink.icon}
          active={pathName === navLink.href}
        />
      </Link>
    ))}
    
  </AppShell.Navbar>
  )
}
